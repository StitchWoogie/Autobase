"""
Length-prefixed JSON protocol.
Wire format: 4-byte little-endian uint32 (body length) + UTF-8 JSON body.
Compatible with C# JsonLengthPrefixSerializer.
"""

import struct
import json
import asyncio

MAX_MESSAGE_SIZE = 10_000_000  # 10MB


async def read_message(reader: asyncio.StreamReader) -> dict:
    """Read a length-prefixed JSON message from the stream."""
    len_buf = await reader.readexactly(4)
    length = struct.unpack('<I', len_buf)[0]

    if length <= 0 or length > MAX_MESSAGE_SIZE:
        raise ValueError(f"Invalid message length: {length}")

    body = await reader.readexactly(length)
    return json.loads(body.decode('utf-8'))


async def write_message(writer: asyncio.StreamWriter, msg: dict):
    """Write a length-prefixed JSON message to the stream."""
    body = json.dumps(msg, ensure_ascii=False, default=str).encode('utf-8')
    header = struct.pack('<I', len(body))
    writer.write(header + body)
    await writer.drain()

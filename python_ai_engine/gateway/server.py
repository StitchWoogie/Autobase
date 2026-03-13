"""
Asyncio TCP Gateway Server.
Accepts SCADA Core connections and routes requests to services.
"""

import asyncio
import logging

from .protocol import read_message, write_message

logger = logging.getLogger(__name__)


class GatewayServer:
    def __init__(self, host: str, port: int, router):
        self.host = host
        self.port = port
        self.router = router
        self._server = None

    async def start(self):
        self._server = await asyncio.start_server(
            self._handle_client, self.host, self.port
        )
        logger.info("Python AI Engine listening on %s:%d", self.host, self.port)

    async def _handle_client(self, reader: asyncio.StreamReader, writer: asyncio.StreamWriter):
        addr = writer.get_extra_info('peername')
        logger.info("Client connected: %s", addr)
        try:
            while True:
                msg = await read_message(reader)
                response = await self.router.route(msg)
                await write_message(writer, response)
        except asyncio.IncompleteReadError:
            logger.info("Client disconnected (EOF): %s", addr)
        except ConnectionResetError:
            logger.info("Client disconnected (reset): %s", addr)
        except Exception as e:
            logger.error("Error handling client %s: %s", addr, e)
        finally:
            try:
                writer.close()
                await writer.wait_closed()
            except Exception:
                pass

    async def stop(self):
        if self._server:
            self._server.close()
            await self._server.wait_closed()
            logger.info("Gateway server stopped.")

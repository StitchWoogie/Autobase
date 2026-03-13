using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUA.Client.Core
{
    public static class OpcUaTypeHelper
    {
        public static Type ToSystemType(BuiltInType btype)
        {
            switch (btype)
            {
                case BuiltInType.Boolean: return typeof(bool);
                case BuiltInType.SByte: return typeof(sbyte);
                case BuiltInType.Byte: return typeof(byte);
                case BuiltInType.Int16: return typeof(short);
                case BuiltInType.UInt16: return typeof(ushort);
                case BuiltInType.Int32: return typeof(int);
                case BuiltInType.UInt32: return typeof(uint);
                case BuiltInType.Int64: return typeof(long);
                case BuiltInType.UInt64: return typeof(ulong);
                case BuiltInType.Float: return typeof(float);
                case BuiltInType.Double: return typeof(double);
                case BuiltInType.String: return typeof(string);
                case BuiltInType.DateTime: return typeof(DateTime);
                case BuiltInType.Guid: return typeof(Guid);
                case BuiltInType.ByteString: return typeof(byte[]);
                case BuiltInType.XmlElement: return typeof(System.Xml.XmlElement);
                case BuiltInType.NodeId: return typeof(NodeId);
                case BuiltInType.ExpandedNodeId: return typeof(ExpandedNodeId);
                case BuiltInType.StatusCode: return typeof(StatusCode);
                case BuiltInType.QualifiedName: return typeof(QualifiedName);
                case BuiltInType.LocalizedText: return typeof(LocalizedText);
                case BuiltInType.ExtensionObject: return typeof(ExtensionObject);
                case BuiltInType.DataValue: return typeof(DataValue);
                case BuiltInType.Variant: return typeof(Variant);
                case BuiltInType.DiagnosticInfo: return typeof(DiagnosticInfo);
                default: return typeof(object);
            }
        }
    }
}

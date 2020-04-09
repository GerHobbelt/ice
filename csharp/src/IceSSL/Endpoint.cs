//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using System.Collections.Generic;

namespace IceSSL
{
    /// <summary> Endpoint represents a TLS layer over another endpoint.</summary>
    public sealed class Endpoint : Ice.Endpoint
    {
        public override string ConnectionId => _delegate.ConnectionId;
        public override bool HasCompressionFlag => _delegate.HasCompressionFlag;
        public override bool IsDatagram => _delegate.IsDatagram;
        public override bool IsSecure => _delegate.IsSecure;
        public override int Timeout => _delegate.Timeout;
        public override Ice.EndpointType Type => _delegate.Type;
        public override Ice.Endpoint Underlying => _delegate;

        private readonly Ice.Endpoint _delegate;
        private readonly Instance _instance;

        public override bool Equals(Ice.Endpoint? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (other is Endpoint sslEndpoint)
            {
                return _delegate.Equals(sslEndpoint._delegate);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => _delegate.GetHashCode();

        public override string OptionsToString() => _delegate.OptionsToString();

        public override bool Equivalent(Ice.Endpoint endpoint)
        {
            if (endpoint is Endpoint sslEndpoint)
            {
                return _delegate.Equivalent(sslEndpoint._delegate);
            }
            else
            {
                return false;
            }
        }

        public override void IceWriteImpl(Ice.OutputStream ostr) => _delegate.IceWriteImpl(ostr);

        public override Ice.Endpoint NewCompressionFlag(bool compressionFlag) =>
            compressionFlag == _delegate.HasCompressionFlag ? this :
                new Endpoint(_instance, _delegate.NewCompressionFlag(compressionFlag));

        public override Ice.Endpoint NewConnectionId(string connectionId) =>
            connectionId == _delegate.ConnectionId ? this :
                new Endpoint(_instance, _delegate.NewConnectionId(connectionId));

        public override Ice.Endpoint NewTimeout(int timeout) =>
        timeout == _delegate.Timeout ? this : new Endpoint(_instance, _delegate.NewTimeout(timeout));

        public override void ConnectorsAsync(Ice.EndpointSelectionType endpointSelection,
                                             Ice.IEndpointConnectors callback)
        {
            string host = "";
            for (Ice.Endpoint? p = _delegate; p != null; p = p.Underlying)
            {
                if (p is Ice.IPEndpoint ipEndpoint)
                {
                    host = ipEndpoint.Host;
                    break;
                }
            }
            _delegate.ConnectorsAsync(endpointSelection, new EndpointI_connectorsI(_instance, host, callback));
        }

        public override List<Ice.Endpoint> ExpandHost(out Ice.Endpoint? publish)
        {
            var endpoints = new List<Ice.Endpoint>();
            foreach (Ice.Endpoint e in _delegate.ExpandHost(out publish))
            {
                endpoints.Add(e == _delegate ? this : new Endpoint(_instance, e));
            }
            if (publish != null)
            {
                publish = publish == _delegate ? this : new Endpoint(_instance, publish);
            }
            return endpoints;
        }

        public override List<Ice.Endpoint> ExpandIfWildcard()
        {
            var l = new List<Ice.Endpoint>();
            foreach (Ice.Endpoint e in _delegate.ExpandIfWildcard())
            {
                l.Add(e == _delegate ? this : new Endpoint(_instance, e));
            }
            return l;
        }

        public override IceInternal.IAcceptor GetAcceptor(string adapterName) =>
            new Acceptor(this, _instance, _delegate.GetAcceptor(adapterName)!, adapterName);

        public override IceInternal.ITransceiver? GetTransceiver() => null;

        internal Endpoint GetEndpoint(Ice.Endpoint del) => del == _delegate ? this : new Endpoint(_instance, del);

        internal Endpoint(Instance instance, Ice.Endpoint del)
        {
            _instance = instance;
            _delegate = del;
        }

        private sealed class EndpointI_connectorsI : Ice.IEndpointConnectors
        {
            private readonly Instance _instance;
            private readonly string _host;
            private readonly Ice.IEndpointConnectors _callback;

            public void Connectors(List<IceInternal.IConnector> connectors)
            {
                var l = new List<IceInternal.IConnector>();
                foreach (IceInternal.IConnector c in connectors)
                {
                    l.Add(new ConnectorI(_instance, c, _host));
                }
                _callback.Connectors(l);
            }
            public void Exception(System.Exception ex) => _callback.Exception(ex);

            internal EndpointI_connectorsI(Instance instance, string host, Ice.IEndpointConnectors cb)
            {
                _instance = instance;
                _host = host;
                _callback = cb;
            }
        }
    }

    internal sealed class EndpointFactoryI : IceInternal.EndpointFactoryWithUnderlying
    {
        private readonly Instance _instance;

        public EndpointFactoryI(Instance instance, Ice.EndpointType type)
            : base(instance, type) =>
            _instance = instance;

        public override IceInternal.IEndpointFactory CloneWithUnderlying(IceInternal.TransportInstance inst,
            Ice.EndpointType underlying) =>
            new EndpointFactoryI(new Instance(_instance.Engine(), inst.Type, inst.Transport), underlying);

        protected override Ice.Endpoint CreateWithUnderlying(Ice.Endpoint underlying, string endpointString,
            Dictionary<string, string?> options, bool oaEndpoint) => new Endpoint(_instance, underlying);

        protected override Ice.Endpoint ReadWithUnderlying(Ice.Endpoint underlying, Ice.InputStream istr) =>
            new Endpoint(_instance, underlying);
    }
}
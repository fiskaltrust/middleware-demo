using fiskaltrust.ifPOS.v0;
using System;
using System.ServiceModel;

namespace csConsoleApplicationSCU
{
    public static class ProxyFactory
    {
        public static IATSSCD CreateProxy(ScuOptions options)
        {
            IATSSCD proxy;

            if (options.ServiceUrl.StartsWith("https://"))
            {
                proxy = GenerateHttpsProxy(options);
            }
            else if (options.ServiceUrl.StartsWith("http://"))
            {
                proxy = GenerateHttpProxy(options);
            }
            else if (options.ServiceUrl.StartsWith("net.pipe://"))
            {
                proxy = GenerateNetPipeProxy(options);
            }
            else if (options.ServiceUrl.StartsWith("net.tcp://"))
            {
                proxy = GenerateNetTcpProxy(options);
            }
            else
            {
                throw new ArgumentException();
            }

            return proxy;
        }

        public static IATSSCD GenerateNetTcpProxy(ScuOptions options)
        {
            IATSSCD proxy;
            var binding = new NetTcpBinding(SecurityMode.None);
            var factory = new ChannelFactory<IATSSCD>(binding, new EndpointAddress(options.ServiceUrl));
            proxy = factory.CreateChannel();
            return proxy;
        }

        public static IATSSCD GenerateNetPipeProxy(ScuOptions options)
        {
            IATSSCD proxy;
            //this is loc
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var factory = new ChannelFactory<IATSSCD>(binding, new EndpointAddress(options.ServiceUrl));
            proxy = factory.CreateChannel();
            return proxy;
        }

        public static IATSSCD GenerateHttpProxy(ScuOptions options)
        {
            IATSSCD proxy;
            //var binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            //var factory = new ChannelFactory<IATSSCD>(binding, new EndpointAddress(url));
            //proxy = factory.CreateChannel();
            var binding = new BasicHttpBinding
            {
                Security =
                                {
                                    Mode= BasicHttpSecurityMode.TransportCredentialOnly,
                                    Transport=
                                    {
                                ClientCredentialType= HttpClientCredentialType.None
                                         //ClientCredentialType= HttpClientCredentialType.Basic
                                    }
                                }
            };
            var factory = new ChannelFactory<IATSSCD>(binding, new EndpointAddress(options.ServiceUrl));
            factory.Credentials.UserName.UserName = options.CashboxId.ToString();
            factory.Credentials.UserName.Password = options.AccessToken;
            proxy = factory.CreateChannel();
            return proxy;
        }

        public static IATSSCD GenerateHttpsProxy(ScuOptions options)
        {
            var binding = new BasicHttpBinding
            {
                Security =
                                {
                                    Mode= BasicHttpSecurityMode.Transport,
                                    Transport=
                                    {
                                         ClientCredentialType= HttpClientCredentialType.Basic
                                    }
                                }
            };
            var factory = new ChannelFactory<IATSSCD>(binding, new EndpointAddress(options.ServiceUrl));
            factory.Credentials.UserName.UserName = options.CashboxId.ToString();
            factory.Credentials.UserName.Password = options.AccessToken;
            return factory.CreateChannel();
        }
    }
}

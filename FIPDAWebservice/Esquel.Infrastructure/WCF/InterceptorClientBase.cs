using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace Esquel.Library.Infrastructure.WCF
{
    public abstract class InterceptorClientBase<T> : ClientBase<T> where T : class
    {
        public InterceptorClientBase()
        {
            Endpoint.Behaviors.Add(new ClientInterceptor(this));
        }
        public InterceptorClientBase(string endpointName)
            : base(endpointName)
        {
            Endpoint.Behaviors.Add(new ClientInterceptor(this));
        }

        public InterceptorClientBase(string endpointName, string remoteAddress)
            : base(endpointName, remoteAddress)
        {
            Endpoint.Behaviors.Add(new ClientInterceptor(this));
        }
        public InterceptorClientBase(string endpointName, EndpointAddress remoteAddress)
            : base(endpointName, remoteAddress)
        {
            Endpoint.Behaviors.Add(new ClientInterceptor(this));
        }
        public InterceptorClientBase(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
            Endpoint.Behaviors.Add(new ClientInterceptor(this));
        }

        protected virtual void PreInvoke(ref Message request)
        { }

        protected virtual void PostInvoke(ref Message reply)
        { }

        class ClientInterceptor : IEndpointBehavior, IClientMessageInspector
        {
            InterceptorClientBase<T> Proxy
            { get; set; }

            internal ClientInterceptor(InterceptorClientBase<T> proxy)
            {
                Proxy = proxy;
            }

            object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                Proxy.PreInvoke(ref request);
                return null;
            }
            void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
            {
                Proxy.PostInvoke(ref reply);
            }

            void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(this);
            }

            void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            { }
            void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            { }
            void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
            { }
        }
    }
}

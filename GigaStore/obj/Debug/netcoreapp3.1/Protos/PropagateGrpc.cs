// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/propagate.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace GigaStore {
  public static partial class Propagate
  {
    static readonly string __ServiceName = "Propagate";

    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    static readonly grpc::Marshaller<global::GigaStore.PropagateRequest> __Marshaller_PropagateRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.PropagateRequest.Parser));
    static readonly grpc::Marshaller<global::GigaStore.PropagateReply> __Marshaller_PropagateReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.PropagateReply.Parser));
    static readonly grpc::Marshaller<global::GigaStore.LockRequest> __Marshaller_LockRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.LockRequest.Parser));
    static readonly grpc::Marshaller<global::GigaStore.LockReply> __Marshaller_LockReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.LockReply.Parser));
    static readonly grpc::Marshaller<global::GigaStore.ChangeRequest> __Marshaller_ChangeRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ChangeRequest.Parser));
    static readonly grpc::Marshaller<global::GigaStore.ChangeReply> __Marshaller_ChangeReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ChangeReply.Parser));
    static readonly grpc::Marshaller<global::GigaStore.ChangeNotificationRequest> __Marshaller_ChangeNotificationRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ChangeNotificationRequest.Parser));
    static readonly grpc::Marshaller<global::GigaStore.ReplicateNewRequest> __Marshaller_ReplicateNewRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ReplicateNewRequest.Parser));
    static readonly grpc::Marshaller<global::GigaStore.ReplicateNewReply> __Marshaller_ReplicateNewReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ReplicateNewReply.Parser));
    static readonly grpc::Marshaller<global::GigaStore.ReplicateRequest> __Marshaller_ReplicateRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ReplicateRequest.Parser));
    static readonly grpc::Marshaller<global::GigaStore.ReplicateReply> __Marshaller_ReplicateReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ReplicateReply.Parser));

    static readonly grpc::Method<global::GigaStore.PropagateRequest, global::GigaStore.PropagateReply> __Method_PropagateServers = new grpc::Method<global::GigaStore.PropagateRequest, global::GigaStore.PropagateReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "PropagateServers",
        __Marshaller_PropagateRequest,
        __Marshaller_PropagateReply);

    static readonly grpc::Method<global::GigaStore.LockRequest, global::GigaStore.LockReply> __Method_LockServers = new grpc::Method<global::GigaStore.LockRequest, global::GigaStore.LockReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "LockServers",
        __Marshaller_LockRequest,
        __Marshaller_LockReply);

    static readonly grpc::Method<global::GigaStore.PropagateRequest, global::GigaStore.PropagateReply> __Method_PropagateServersAdvanced = new grpc::Method<global::GigaStore.PropagateRequest, global::GigaStore.PropagateReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "PropagateServersAdvanced",
        __Marshaller_PropagateRequest,
        __Marshaller_PropagateReply);

    static readonly grpc::Method<global::GigaStore.ChangeRequest, global::GigaStore.ChangeReply> __Method_ChangeMaster = new grpc::Method<global::GigaStore.ChangeRequest, global::GigaStore.ChangeReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ChangeMaster",
        __Marshaller_ChangeRequest,
        __Marshaller_ChangeReply);

    static readonly grpc::Method<global::GigaStore.ChangeNotificationRequest, global::GigaStore.ChangeReply> __Method_ChangeMasterNotification = new grpc::Method<global::GigaStore.ChangeNotificationRequest, global::GigaStore.ChangeReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ChangeMasterNotification",
        __Marshaller_ChangeNotificationRequest,
        __Marshaller_ChangeReply);

    static readonly grpc::Method<global::GigaStore.ReplicateNewRequest, global::GigaStore.ReplicateNewReply> __Method_ReplicateNew = new grpc::Method<global::GigaStore.ReplicateNewRequest, global::GigaStore.ReplicateNewReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ReplicateNew",
        __Marshaller_ReplicateNewRequest,
        __Marshaller_ReplicateNewReply);

    static readonly grpc::Method<global::GigaStore.ReplicateRequest, global::GigaStore.ReplicateReply> __Method_ReplicatePartition = new grpc::Method<global::GigaStore.ReplicateRequest, global::GigaStore.ReplicateReply>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "ReplicatePartition",
        __Marshaller_ReplicateRequest,
        __Marshaller_ReplicateReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::GigaStore.PropagateReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Propagate</summary>
    [grpc::BindServiceMethod(typeof(Propagate), "BindService")]
    public abstract partial class PropagateBase
    {
      public virtual global::System.Threading.Tasks.Task<global::GigaStore.PropagateReply> PropagateServers(global::GigaStore.PropagateRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::GigaStore.LockReply> LockServers(global::GigaStore.LockRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::GigaStore.PropagateReply> PropagateServersAdvanced(global::GigaStore.PropagateRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::GigaStore.ChangeReply> ChangeMaster(global::GigaStore.ChangeRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::GigaStore.ChangeReply> ChangeMasterNotification(global::GigaStore.ChangeNotificationRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::GigaStore.ReplicateNewReply> ReplicateNew(global::GigaStore.ReplicateNewRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task ReplicatePartition(global::GigaStore.ReplicateRequest request, grpc::IServerStreamWriter<global::GigaStore.ReplicateReply> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Propagate</summary>
    public partial class PropagateClient : grpc::ClientBase<PropagateClient>
    {
      /// <summary>Creates a new client for Propagate</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public PropagateClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Propagate that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public PropagateClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected PropagateClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected PropagateClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::GigaStore.PropagateReply PropagateServers(global::GigaStore.PropagateRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PropagateServers(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GigaStore.PropagateReply PropagateServers(global::GigaStore.PropagateRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_PropagateServers, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.PropagateReply> PropagateServersAsync(global::GigaStore.PropagateRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PropagateServersAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.PropagateReply> PropagateServersAsync(global::GigaStore.PropagateRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_PropagateServers, null, options, request);
      }
      public virtual global::GigaStore.LockReply LockServers(global::GigaStore.LockRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return LockServers(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GigaStore.LockReply LockServers(global::GigaStore.LockRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_LockServers, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.LockReply> LockServersAsync(global::GigaStore.LockRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return LockServersAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.LockReply> LockServersAsync(global::GigaStore.LockRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_LockServers, null, options, request);
      }
      public virtual global::GigaStore.PropagateReply PropagateServersAdvanced(global::GigaStore.PropagateRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PropagateServersAdvanced(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GigaStore.PropagateReply PropagateServersAdvanced(global::GigaStore.PropagateRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_PropagateServersAdvanced, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.PropagateReply> PropagateServersAdvancedAsync(global::GigaStore.PropagateRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PropagateServersAdvancedAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.PropagateReply> PropagateServersAdvancedAsync(global::GigaStore.PropagateRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_PropagateServersAdvanced, null, options, request);
      }
      public virtual global::GigaStore.ChangeReply ChangeMaster(global::GigaStore.ChangeRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ChangeMaster(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GigaStore.ChangeReply ChangeMaster(global::GigaStore.ChangeRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_ChangeMaster, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.ChangeReply> ChangeMasterAsync(global::GigaStore.ChangeRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ChangeMasterAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.ChangeReply> ChangeMasterAsync(global::GigaStore.ChangeRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_ChangeMaster, null, options, request);
      }
      public virtual global::GigaStore.ChangeReply ChangeMasterNotification(global::GigaStore.ChangeNotificationRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ChangeMasterNotification(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GigaStore.ChangeReply ChangeMasterNotification(global::GigaStore.ChangeNotificationRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_ChangeMasterNotification, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.ChangeReply> ChangeMasterNotificationAsync(global::GigaStore.ChangeNotificationRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ChangeMasterNotificationAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.ChangeReply> ChangeMasterNotificationAsync(global::GigaStore.ChangeNotificationRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_ChangeMasterNotification, null, options, request);
      }
      public virtual global::GigaStore.ReplicateNewReply ReplicateNew(global::GigaStore.ReplicateNewRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ReplicateNew(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GigaStore.ReplicateNewReply ReplicateNew(global::GigaStore.ReplicateNewRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_ReplicateNew, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.ReplicateNewReply> ReplicateNewAsync(global::GigaStore.ReplicateNewRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ReplicateNewAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.ReplicateNewReply> ReplicateNewAsync(global::GigaStore.ReplicateNewRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_ReplicateNew, null, options, request);
      }
      public virtual grpc::AsyncServerStreamingCall<global::GigaStore.ReplicateReply> ReplicatePartition(global::GigaStore.ReplicateRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ReplicatePartition(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncServerStreamingCall<global::GigaStore.ReplicateReply> ReplicatePartition(global::GigaStore.ReplicateRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_ReplicatePartition, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override PropagateClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new PropagateClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(PropagateBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_PropagateServers, serviceImpl.PropagateServers)
          .AddMethod(__Method_LockServers, serviceImpl.LockServers)
          .AddMethod(__Method_PropagateServersAdvanced, serviceImpl.PropagateServersAdvanced)
          .AddMethod(__Method_ChangeMaster, serviceImpl.ChangeMaster)
          .AddMethod(__Method_ChangeMasterNotification, serviceImpl.ChangeMasterNotification)
          .AddMethod(__Method_ReplicateNew, serviceImpl.ReplicateNew)
          .AddMethod(__Method_ReplicatePartition, serviceImpl.ReplicatePartition).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, PropagateBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_PropagateServers, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::GigaStore.PropagateRequest, global::GigaStore.PropagateReply>(serviceImpl.PropagateServers));
      serviceBinder.AddMethod(__Method_LockServers, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::GigaStore.LockRequest, global::GigaStore.LockReply>(serviceImpl.LockServers));
      serviceBinder.AddMethod(__Method_PropagateServersAdvanced, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::GigaStore.PropagateRequest, global::GigaStore.PropagateReply>(serviceImpl.PropagateServersAdvanced));
      serviceBinder.AddMethod(__Method_ChangeMaster, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::GigaStore.ChangeRequest, global::GigaStore.ChangeReply>(serviceImpl.ChangeMaster));
      serviceBinder.AddMethod(__Method_ChangeMasterNotification, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::GigaStore.ChangeNotificationRequest, global::GigaStore.ChangeReply>(serviceImpl.ChangeMasterNotification));
      serviceBinder.AddMethod(__Method_ReplicateNew, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::GigaStore.ReplicateNewRequest, global::GigaStore.ReplicateNewReply>(serviceImpl.ReplicateNew));
      serviceBinder.AddMethod(__Method_ReplicatePartition, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::GigaStore.ReplicateRequest, global::GigaStore.ReplicateReply>(serviceImpl.ReplicatePartition));
    }

  }
}
#endregion

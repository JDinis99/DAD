// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: giga.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace GigaStore {
  public static partial class Giga
  {
    static readonly string __ServiceName = "Giga";

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

    static readonly grpc::Marshaller<global::GigaStore.ReadRequest> __Marshaller_ReadRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ReadRequest.Parser));
    static readonly grpc::Marshaller<global::GigaStore.ReadReply> __Marshaller_ReadReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.ReadReply.Parser));
    static readonly grpc::Marshaller<global::GigaStore.WriteRequest> __Marshaller_WriteRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.WriteRequest.Parser));
    static readonly grpc::Marshaller<global::GigaStore.WriteReply> __Marshaller_WriteReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GigaStore.WriteReply.Parser));

    static readonly grpc::Method<global::GigaStore.ReadRequest, global::GigaStore.ReadReply> __Method_Read = new grpc::Method<global::GigaStore.ReadRequest, global::GigaStore.ReadReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Read",
        __Marshaller_ReadRequest,
        __Marshaller_ReadReply);

    static readonly grpc::Method<global::GigaStore.WriteRequest, global::GigaStore.WriteReply> __Method_Write = new grpc::Method<global::GigaStore.WriteRequest, global::GigaStore.WriteReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Write",
        __Marshaller_WriteRequest,
        __Marshaller_WriteReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::GigaStore.GigaReflection.Descriptor.Services[0]; }
    }

    /// <summary>Client for Giga</summary>
    public partial class GigaClient : grpc::ClientBase<GigaClient>
    {
      /// <summary>Creates a new client for Giga</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public GigaClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Giga that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public GigaClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected GigaClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected GigaClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::GigaStore.ReadReply Read(global::GigaStore.ReadRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Read(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GigaStore.ReadReply Read(global::GigaStore.ReadRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Read, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.ReadReply> ReadAsync(global::GigaStore.ReadRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return ReadAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.ReadReply> ReadAsync(global::GigaStore.ReadRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Read, null, options, request);
      }
      public virtual global::GigaStore.WriteReply Write(global::GigaStore.WriteRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Write(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::GigaStore.WriteReply Write(global::GigaStore.WriteRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Write, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.WriteReply> WriteAsync(global::GigaStore.WriteRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return WriteAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::GigaStore.WriteReply> WriteAsync(global::GigaStore.WriteRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Write, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override GigaClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new GigaClient(configuration);
      }
    }

  }
}
#endregion
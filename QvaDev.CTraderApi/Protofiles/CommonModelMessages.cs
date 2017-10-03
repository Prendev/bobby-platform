// Generated by ProtoGen, Version=2.4.1.555, Culture=neutral, PublicKeyToken=55f7125234beb589.  DO NOT EDIT!
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
public static partial class CommonModelMessages {

  #region Extension registration
  public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
  }
  #endregion
  #region Static variables
  internal static pbd::MessageDescriptor internal__static_ProtoIntRange__Descriptor;
  internal static pb::FieldAccess.FieldAccessorTable<global::ProtoIntRange, global::ProtoIntRange.Builder> internal__static_ProtoIntRange__FieldAccessorTable;
  internal static pbd::MessageDescriptor internal__static_ProtoLongRange__Descriptor;
  internal static pb::FieldAccess.FieldAccessorTable<global::ProtoLongRange, global::ProtoLongRange.Builder> internal__static_ProtoLongRange__FieldAccessorTable;
  internal static pbd::MessageDescriptor internal__static_ProtoDoubleRange__Descriptor;
  internal static pb::FieldAccess.FieldAccessorTable<global::ProtoDoubleRange, global::ProtoDoubleRange.Builder> internal__static_ProtoDoubleRange__FieldAccessorTable;
  #endregion
  #region Descriptor
  public static pbd::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbd::FileDescriptor descriptor;
  
  static CommonModelMessages() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChlDb21tb25Nb2RlbE1lc3NhZ2VzLnByb3RvIikKDVByb3RvSW50UmFuZ2US", 
          "DAoEZnJvbRgBIAEoBRIKCgJ0bxgCIAEoBSIqCg5Qcm90b0xvbmdSYW5nZRIM", 
          "CgRmcm9tGAEgASgDEgoKAnRvGAIgASgDIiwKEFByb3RvRG91YmxlUmFuZ2US", 
          "DAoEZnJvbRgBIAEoARIKCgJ0bxgCIAEoASpSChBQcm90b1BheWxvYWRUeXBl", 
          "Eg0KCUVSUk9SX1JFUxAyEhMKD0hFQVJUQkVBVF9FVkVOVBAzEgwKCFBJTkdf", 
          "UkVREDQSDAoIUElOR19SRVMQNSrqAQoOUHJvdG9FcnJvckNvZGUSEQoNVU5L", 
          "Tk9XTl9FUlJPUhABEhcKE1VOU1VQUE9SVEVEX01FU1NBR0UQAhITCg9JTlZB", 
          "TElEX1JFUVVFU1QQAxISCg5XUk9OR19QQVNTV09SRBAEEhEKDVRJTUVPVVRf", 
          "RVJST1IQBRIUChBFTlRJVFlfTk9UX0ZPVU5EEAYSFgoSQ0FOVF9ST1VURV9S", 
          "RVFVRVNUEAcSEgoORlJBTUVfVE9PX0xPTkcQCBIRCg1NQVJLRVRfQ0xPU0VE", 
          "EAkSGwoXQ09OQ1VSUkVOVF9NT0RJRklDQVRJT04QCkJNCihjb20ueHRyYWRl", 
          "ci5wcm90b2NvbC5wcm90by5jb21tb25zLm1vZGVsQhxDb250YWluZXJDb21t", 
        "b25Nb2RlbE1lc3NhZ2VzUAGgAQE="));
    pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
      descriptor = root;
      internal__static_ProtoIntRange__Descriptor = Descriptor.MessageTypes[0];
      internal__static_ProtoIntRange__FieldAccessorTable = 
          new pb::FieldAccess.FieldAccessorTable<global::ProtoIntRange, global::ProtoIntRange.Builder>(internal__static_ProtoIntRange__Descriptor,
              new string[] { "From", "To", });
      internal__static_ProtoLongRange__Descriptor = Descriptor.MessageTypes[1];
      internal__static_ProtoLongRange__FieldAccessorTable = 
          new pb::FieldAccess.FieldAccessorTable<global::ProtoLongRange, global::ProtoLongRange.Builder>(internal__static_ProtoLongRange__Descriptor,
              new string[] { "From", "To", });
      internal__static_ProtoDoubleRange__Descriptor = Descriptor.MessageTypes[2];
      internal__static_ProtoDoubleRange__FieldAccessorTable = 
          new pb::FieldAccess.FieldAccessorTable<global::ProtoDoubleRange, global::ProtoDoubleRange.Builder>(internal__static_ProtoDoubleRange__Descriptor,
              new string[] { "From", "To", });
      return null;
    };
    pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
        new pbd::FileDescriptor[] {
        }, assigner);
  }
  #endregion
  
}
#region Enums
public enum ProtoPayloadType {
  ERROR_RES = 50,
  HEARTBEAT_EVENT = 51,
  PING_REQ = 52,
  PING_RES = 53,
}

public enum ProtoErrorCode {
  UNKNOWN_ERROR = 1,
  UNSUPPORTED_MESSAGE = 2,
  INVALID_REQUEST = 3,
  WRONG_PASSWORD = 4,
  TIMEOUT_ERROR = 5,
  ENTITY_NOT_FOUND = 6,
  CANT_ROUTE_REQUEST = 7,
  FRAME_TOO_LONG = 8,
  MARKET_CLOSED = 9,
  CONCURRENT_MODIFICATION = 10,
}

#endregion

#region Messages
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
public sealed partial class ProtoIntRange : pb::GeneratedMessage<ProtoIntRange, ProtoIntRange.Builder> {
  private ProtoIntRange() { }
  private static readonly ProtoIntRange defaultInstance = new ProtoIntRange().MakeReadOnly();
  private static readonly string[] _protoIntRangeFieldNames = new string[] { "from", "to" };
  private static readonly uint[] _protoIntRangeFieldTags = new uint[] { 8, 16 };
  public static ProtoIntRange DefaultInstance {
    get { return defaultInstance; }
  }
  
  public override ProtoIntRange DefaultInstanceForType {
    get { return DefaultInstance; }
  }
  
  protected override ProtoIntRange ThisMessage {
    get { return this; }
  }
  
  public static pbd::MessageDescriptor Descriptor {
    get { return global::CommonModelMessages.internal__static_ProtoIntRange__Descriptor; }
  }
  
  protected override pb::FieldAccess.FieldAccessorTable<ProtoIntRange, ProtoIntRange.Builder> InternalFieldAccessors {
    get { return global::CommonModelMessages.internal__static_ProtoIntRange__FieldAccessorTable; }
  }
  
  public const int FromFieldNumber = 1;
  private bool hasFrom;
  private int from_;
  public bool HasFrom {
    get { return hasFrom; }
  }
  public int From {
    get { return from_; }
  }
  
  public const int ToFieldNumber = 2;
  private bool hasTo;
  private int to_;
  public bool HasTo {
    get { return hasTo; }
  }
  public int To {
    get { return to_; }
  }
  
  public override bool IsInitialized {
    get {
      return true;
    }
  }
  
  public override void WriteTo(pb::ICodedOutputStream output) {
    CalcSerializedSize();
    string[] field_names = _protoIntRangeFieldNames;
    if (hasFrom) {
      output.WriteInt32(1, field_names[0], From);
    }
    if (hasTo) {
      output.WriteInt32(2, field_names[1], To);
    }
    UnknownFields.WriteTo(output);
  }
  
  private int memoizedSerializedSize = -1;
  public override int SerializedSize {
    get {
      int size = memoizedSerializedSize;
      if (size != -1) return size;
      return CalcSerializedSize();
    }
  }
  
  private int CalcSerializedSize() {
    int size = memoizedSerializedSize;
    if (size != -1) return size;
    
    size = 0;
    if (hasFrom) {
      size += pb::CodedOutputStream.ComputeInt32Size(1, From);
    }
    if (hasTo) {
      size += pb::CodedOutputStream.ComputeInt32Size(2, To);
    }
    size += UnknownFields.SerializedSize;
    memoizedSerializedSize = size;
    return size;
  }
  public static ProtoIntRange ParseFrom(pb::ByteString data) {
    return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
  }
  public static ProtoIntRange ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
  }
  public static ProtoIntRange ParseFrom(byte[] data) {
    return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
  }
  public static ProtoIntRange ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
  }
  public static ProtoIntRange ParseFrom(global::System.IO.Stream input) {
    return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
  }
  public static ProtoIntRange ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
  }
  public static ProtoIntRange ParseDelimitedFrom(global::System.IO.Stream input) {
    return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
  }
  public static ProtoIntRange ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
    return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
  }
  public static ProtoIntRange ParseFrom(pb::ICodedInputStream input) {
    return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
  }
  public static ProtoIntRange ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
  }
  private ProtoIntRange MakeReadOnly() {
    return this;
  }
  
  public static Builder CreateBuilder() { return new Builder(); }
  public override Builder ToBuilder() { return CreateBuilder(this); }
  public override Builder CreateBuilderForType() { return new Builder(); }
  public static Builder CreateBuilder(ProtoIntRange prototype) {
    return new Builder(prototype);
  }
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class Builder : pb::GeneratedBuilder<ProtoIntRange, Builder> {
    protected override Builder ThisBuilder {
      get { return this; }
    }
    public Builder() {
      result = DefaultInstance;
      resultIsReadOnly = true;
    }
    internal Builder(ProtoIntRange cloneFrom) {
      result = cloneFrom;
      resultIsReadOnly = true;
    }
    
    private bool resultIsReadOnly;
    private ProtoIntRange result;
    
    private ProtoIntRange PrepareBuilder() {
      if (resultIsReadOnly) {
        ProtoIntRange original = result;
        result = new ProtoIntRange();
        resultIsReadOnly = false;
        MergeFrom(original);
      }
      return result;
    }
    
    public override bool IsInitialized {
      get { return result.IsInitialized; }
    }
    
    protected override ProtoIntRange MessageBeingBuilt {
      get { return PrepareBuilder(); }
    }
    
    public override Builder Clear() {
      result = DefaultInstance;
      resultIsReadOnly = true;
      return this;
    }
    
    public override Builder Clone() {
      if (resultIsReadOnly) {
        return new Builder(result);
      } else {
        return new Builder().MergeFrom(result);
      }
    }
    
    public override pbd::MessageDescriptor DescriptorForType {
      get { return global::ProtoIntRange.Descriptor; }
    }
    
    public override ProtoIntRange DefaultInstanceForType {
      get { return global::ProtoIntRange.DefaultInstance; }
    }
    
    public override ProtoIntRange BuildPartial() {
      if (resultIsReadOnly) {
        return result;
      }
      resultIsReadOnly = true;
      return result.MakeReadOnly();
    }
    
    public override Builder MergeFrom(pb::IMessage other) {
      if (other is ProtoIntRange) {
        return MergeFrom((ProtoIntRange) other);
      } else {
        base.MergeFrom(other);
        return this;
      }
    }
    
    public override Builder MergeFrom(ProtoIntRange other) {
      if (other == global::ProtoIntRange.DefaultInstance) return this;
      PrepareBuilder();
      if (other.HasFrom) {
        From = other.From;
      }
      if (other.HasTo) {
        To = other.To;
      }
      this.MergeUnknownFields(other.UnknownFields);
      return this;
    }
    
    public override Builder MergeFrom(pb::ICodedInputStream input) {
      return MergeFrom(input, pb::ExtensionRegistry.Empty);
    }
    
    public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      PrepareBuilder();
      pb::UnknownFieldSet.Builder unknownFields = null;
      uint tag;
      string field_name;
      while (input.ReadTag(out tag, out field_name)) {
        if(tag == 0 && field_name != null) {
          int field_ordinal = global::System.Array.BinarySearch(_protoIntRangeFieldNames, field_name, global::System.StringComparer.Ordinal);
          if(field_ordinal >= 0)
            tag = _protoIntRangeFieldTags[field_ordinal];
          else {
            if (unknownFields == null) {
              unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
            }
            ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
            continue;
          }
        }
        switch (tag) {
          case 0: {
            throw pb::InvalidProtocolBufferException.InvalidTag();
          }
          default: {
            if (pb::WireFormat.IsEndGroupTag(tag)) {
              if (unknownFields != null) {
                this.UnknownFields = unknownFields.Build();
              }
              return this;
            }
            if (unknownFields == null) {
              unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
            }
            ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
            break;
          }
          case 8: {
            result.hasFrom = input.ReadInt32(ref result.from_);
            break;
          }
          case 16: {
            result.hasTo = input.ReadInt32(ref result.to_);
            break;
          }
        }
      }
      
      if (unknownFields != null) {
        this.UnknownFields = unknownFields.Build();
      }
      return this;
    }
    
    
    public bool HasFrom {
      get { return result.hasFrom; }
    }
    public int From {
      get { return result.From; }
      set { SetFrom(value); }
    }
    public Builder SetFrom(int value) {
      PrepareBuilder();
      result.hasFrom = true;
      result.from_ = value;
      return this;
    }
    public Builder ClearFrom() {
      PrepareBuilder();
      result.hasFrom = false;
      result.from_ = 0;
      return this;
    }
    
    public bool HasTo {
      get { return result.hasTo; }
    }
    public int To {
      get { return result.To; }
      set { SetTo(value); }
    }
    public Builder SetTo(int value) {
      PrepareBuilder();
      result.hasTo = true;
      result.to_ = value;
      return this;
    }
    public Builder ClearTo() {
      PrepareBuilder();
      result.hasTo = false;
      result.to_ = 0;
      return this;
    }
  }
  static ProtoIntRange() {
    object.ReferenceEquals(global::CommonModelMessages.Descriptor, null);
  }
}

[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
public sealed partial class ProtoLongRange : pb::GeneratedMessage<ProtoLongRange, ProtoLongRange.Builder> {
  private ProtoLongRange() { }
  private static readonly ProtoLongRange defaultInstance = new ProtoLongRange().MakeReadOnly();
  private static readonly string[] _protoLongRangeFieldNames = new string[] { "from", "to" };
  private static readonly uint[] _protoLongRangeFieldTags = new uint[] { 8, 16 };
  public static ProtoLongRange DefaultInstance {
    get { return defaultInstance; }
  }
  
  public override ProtoLongRange DefaultInstanceForType {
    get { return DefaultInstance; }
  }
  
  protected override ProtoLongRange ThisMessage {
    get { return this; }
  }
  
  public static pbd::MessageDescriptor Descriptor {
    get { return global::CommonModelMessages.internal__static_ProtoLongRange__Descriptor; }
  }
  
  protected override pb::FieldAccess.FieldAccessorTable<ProtoLongRange, ProtoLongRange.Builder> InternalFieldAccessors {
    get { return global::CommonModelMessages.internal__static_ProtoLongRange__FieldAccessorTable; }
  }
  
  public const int FromFieldNumber = 1;
  private bool hasFrom;
  private long from_;
  public bool HasFrom {
    get { return hasFrom; }
  }
  public long From {
    get { return from_; }
  }
  
  public const int ToFieldNumber = 2;
  private bool hasTo;
  private long to_;
  public bool HasTo {
    get { return hasTo; }
  }
  public long To {
    get { return to_; }
  }
  
  public override bool IsInitialized {
    get {
      return true;
    }
  }
  
  public override void WriteTo(pb::ICodedOutputStream output) {
    CalcSerializedSize();
    string[] field_names = _protoLongRangeFieldNames;
    if (hasFrom) {
      output.WriteInt64(1, field_names[0], From);
    }
    if (hasTo) {
      output.WriteInt64(2, field_names[1], To);
    }
    UnknownFields.WriteTo(output);
  }
  
  private int memoizedSerializedSize = -1;
  public override int SerializedSize {
    get {
      int size = memoizedSerializedSize;
      if (size != -1) return size;
      return CalcSerializedSize();
    }
  }
  
  private int CalcSerializedSize() {
    int size = memoizedSerializedSize;
    if (size != -1) return size;
    
    size = 0;
    if (hasFrom) {
      size += pb::CodedOutputStream.ComputeInt64Size(1, From);
    }
    if (hasTo) {
      size += pb::CodedOutputStream.ComputeInt64Size(2, To);
    }
    size += UnknownFields.SerializedSize;
    memoizedSerializedSize = size;
    return size;
  }
  public static ProtoLongRange ParseFrom(pb::ByteString data) {
    return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
  }
  public static ProtoLongRange ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
  }
  public static ProtoLongRange ParseFrom(byte[] data) {
    return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
  }
  public static ProtoLongRange ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
  }
  public static ProtoLongRange ParseFrom(global::System.IO.Stream input) {
    return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
  }
  public static ProtoLongRange ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
  }
  public static ProtoLongRange ParseDelimitedFrom(global::System.IO.Stream input) {
    return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
  }
  public static ProtoLongRange ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
    return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
  }
  public static ProtoLongRange ParseFrom(pb::ICodedInputStream input) {
    return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
  }
  public static ProtoLongRange ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
  }
  private ProtoLongRange MakeReadOnly() {
    return this;
  }
  
  public static Builder CreateBuilder() { return new Builder(); }
  public override Builder ToBuilder() { return CreateBuilder(this); }
  public override Builder CreateBuilderForType() { return new Builder(); }
  public static Builder CreateBuilder(ProtoLongRange prototype) {
    return new Builder(prototype);
  }
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class Builder : pb::GeneratedBuilder<ProtoLongRange, Builder> {
    protected override Builder ThisBuilder {
      get { return this; }
    }
    public Builder() {
      result = DefaultInstance;
      resultIsReadOnly = true;
    }
    internal Builder(ProtoLongRange cloneFrom) {
      result = cloneFrom;
      resultIsReadOnly = true;
    }
    
    private bool resultIsReadOnly;
    private ProtoLongRange result;
    
    private ProtoLongRange PrepareBuilder() {
      if (resultIsReadOnly) {
        ProtoLongRange original = result;
        result = new ProtoLongRange();
        resultIsReadOnly = false;
        MergeFrom(original);
      }
      return result;
    }
    
    public override bool IsInitialized {
      get { return result.IsInitialized; }
    }
    
    protected override ProtoLongRange MessageBeingBuilt {
      get { return PrepareBuilder(); }
    }
    
    public override Builder Clear() {
      result = DefaultInstance;
      resultIsReadOnly = true;
      return this;
    }
    
    public override Builder Clone() {
      if (resultIsReadOnly) {
        return new Builder(result);
      } else {
        return new Builder().MergeFrom(result);
      }
    }
    
    public override pbd::MessageDescriptor DescriptorForType {
      get { return global::ProtoLongRange.Descriptor; }
    }
    
    public override ProtoLongRange DefaultInstanceForType {
      get { return global::ProtoLongRange.DefaultInstance; }
    }
    
    public override ProtoLongRange BuildPartial() {
      if (resultIsReadOnly) {
        return result;
      }
      resultIsReadOnly = true;
      return result.MakeReadOnly();
    }
    
    public override Builder MergeFrom(pb::IMessage other) {
      if (other is ProtoLongRange) {
        return MergeFrom((ProtoLongRange) other);
      } else {
        base.MergeFrom(other);
        return this;
      }
    }
    
    public override Builder MergeFrom(ProtoLongRange other) {
      if (other == global::ProtoLongRange.DefaultInstance) return this;
      PrepareBuilder();
      if (other.HasFrom) {
        From = other.From;
      }
      if (other.HasTo) {
        To = other.To;
      }
      this.MergeUnknownFields(other.UnknownFields);
      return this;
    }
    
    public override Builder MergeFrom(pb::ICodedInputStream input) {
      return MergeFrom(input, pb::ExtensionRegistry.Empty);
    }
    
    public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      PrepareBuilder();
      pb::UnknownFieldSet.Builder unknownFields = null;
      uint tag;
      string field_name;
      while (input.ReadTag(out tag, out field_name)) {
        if(tag == 0 && field_name != null) {
          int field_ordinal = global::System.Array.BinarySearch(_protoLongRangeFieldNames, field_name, global::System.StringComparer.Ordinal);
          if(field_ordinal >= 0)
            tag = _protoLongRangeFieldTags[field_ordinal];
          else {
            if (unknownFields == null) {
              unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
            }
            ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
            continue;
          }
        }
        switch (tag) {
          case 0: {
            throw pb::InvalidProtocolBufferException.InvalidTag();
          }
          default: {
            if (pb::WireFormat.IsEndGroupTag(tag)) {
              if (unknownFields != null) {
                this.UnknownFields = unknownFields.Build();
              }
              return this;
            }
            if (unknownFields == null) {
              unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
            }
            ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
            break;
          }
          case 8: {
            result.hasFrom = input.ReadInt64(ref result.from_);
            break;
          }
          case 16: {
            result.hasTo = input.ReadInt64(ref result.to_);
            break;
          }
        }
      }
      
      if (unknownFields != null) {
        this.UnknownFields = unknownFields.Build();
      }
      return this;
    }
    
    
    public bool HasFrom {
      get { return result.hasFrom; }
    }
    public long From {
      get { return result.From; }
      set { SetFrom(value); }
    }
    public Builder SetFrom(long value) {
      PrepareBuilder();
      result.hasFrom = true;
      result.from_ = value;
      return this;
    }
    public Builder ClearFrom() {
      PrepareBuilder();
      result.hasFrom = false;
      result.from_ = 0L;
      return this;
    }
    
    public bool HasTo {
      get { return result.hasTo; }
    }
    public long To {
      get { return result.To; }
      set { SetTo(value); }
    }
    public Builder SetTo(long value) {
      PrepareBuilder();
      result.hasTo = true;
      result.to_ = value;
      return this;
    }
    public Builder ClearTo() {
      PrepareBuilder();
      result.hasTo = false;
      result.to_ = 0L;
      return this;
    }
  }
  static ProtoLongRange() {
    object.ReferenceEquals(global::CommonModelMessages.Descriptor, null);
  }
}

[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
public sealed partial class ProtoDoubleRange : pb::GeneratedMessage<ProtoDoubleRange, ProtoDoubleRange.Builder> {
  private ProtoDoubleRange() { }
  private static readonly ProtoDoubleRange defaultInstance = new ProtoDoubleRange().MakeReadOnly();
  private static readonly string[] _protoDoubleRangeFieldNames = new string[] { "from", "to" };
  private static readonly uint[] _protoDoubleRangeFieldTags = new uint[] { 9, 17 };
  public static ProtoDoubleRange DefaultInstance {
    get { return defaultInstance; }
  }
  
  public override ProtoDoubleRange DefaultInstanceForType {
    get { return DefaultInstance; }
  }
  
  protected override ProtoDoubleRange ThisMessage {
    get { return this; }
  }
  
  public static pbd::MessageDescriptor Descriptor {
    get { return global::CommonModelMessages.internal__static_ProtoDoubleRange__Descriptor; }
  }
  
  protected override pb::FieldAccess.FieldAccessorTable<ProtoDoubleRange, ProtoDoubleRange.Builder> InternalFieldAccessors {
    get { return global::CommonModelMessages.internal__static_ProtoDoubleRange__FieldAccessorTable; }
  }
  
  public const int FromFieldNumber = 1;
  private bool hasFrom;
  private double from_;
  public bool HasFrom {
    get { return hasFrom; }
  }
  public double From {
    get { return from_; }
  }
  
  public const int ToFieldNumber = 2;
  private bool hasTo;
  private double to_;
  public bool HasTo {
    get { return hasTo; }
  }
  public double To {
    get { return to_; }
  }
  
  public override bool IsInitialized {
    get {
      return true;
    }
  }
  
  public override void WriteTo(pb::ICodedOutputStream output) {
    CalcSerializedSize();
    string[] field_names = _protoDoubleRangeFieldNames;
    if (hasFrom) {
      output.WriteDouble(1, field_names[0], From);
    }
    if (hasTo) {
      output.WriteDouble(2, field_names[1], To);
    }
    UnknownFields.WriteTo(output);
  }
  
  private int memoizedSerializedSize = -1;
  public override int SerializedSize {
    get {
      int size = memoizedSerializedSize;
      if (size != -1) return size;
      return CalcSerializedSize();
    }
  }
  
  private int CalcSerializedSize() {
    int size = memoizedSerializedSize;
    if (size != -1) return size;
    
    size = 0;
    if (hasFrom) {
      size += pb::CodedOutputStream.ComputeDoubleSize(1, From);
    }
    if (hasTo) {
      size += pb::CodedOutputStream.ComputeDoubleSize(2, To);
    }
    size += UnknownFields.SerializedSize;
    memoizedSerializedSize = size;
    return size;
  }
  public static ProtoDoubleRange ParseFrom(pb::ByteString data) {
    return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
  }
  public static ProtoDoubleRange ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
  }
  public static ProtoDoubleRange ParseFrom(byte[] data) {
    return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
  }
  public static ProtoDoubleRange ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
  }
  public static ProtoDoubleRange ParseFrom(global::System.IO.Stream input) {
    return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
  }
  public static ProtoDoubleRange ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
  }
  public static ProtoDoubleRange ParseDelimitedFrom(global::System.IO.Stream input) {
    return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
  }
  public static ProtoDoubleRange ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
    return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
  }
  public static ProtoDoubleRange ParseFrom(pb::ICodedInputStream input) {
    return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
  }
  public static ProtoDoubleRange ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
    return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
  }
  private ProtoDoubleRange MakeReadOnly() {
    return this;
  }
  
  public static Builder CreateBuilder() { return new Builder(); }
  public override Builder ToBuilder() { return CreateBuilder(this); }
  public override Builder CreateBuilderForType() { return new Builder(); }
  public static Builder CreateBuilder(ProtoDoubleRange prototype) {
    return new Builder(prototype);
  }
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class Builder : pb::GeneratedBuilder<ProtoDoubleRange, Builder> {
    protected override Builder ThisBuilder {
      get { return this; }
    }
    public Builder() {
      result = DefaultInstance;
      resultIsReadOnly = true;
    }
    internal Builder(ProtoDoubleRange cloneFrom) {
      result = cloneFrom;
      resultIsReadOnly = true;
    }
    
    private bool resultIsReadOnly;
    private ProtoDoubleRange result;
    
    private ProtoDoubleRange PrepareBuilder() {
      if (resultIsReadOnly) {
        ProtoDoubleRange original = result;
        result = new ProtoDoubleRange();
        resultIsReadOnly = false;
        MergeFrom(original);
      }
      return result;
    }
    
    public override bool IsInitialized {
      get { return result.IsInitialized; }
    }
    
    protected override ProtoDoubleRange MessageBeingBuilt {
      get { return PrepareBuilder(); }
    }
    
    public override Builder Clear() {
      result = DefaultInstance;
      resultIsReadOnly = true;
      return this;
    }
    
    public override Builder Clone() {
      if (resultIsReadOnly) {
        return new Builder(result);
      } else {
        return new Builder().MergeFrom(result);
      }
    }
    
    public override pbd::MessageDescriptor DescriptorForType {
      get { return global::ProtoDoubleRange.Descriptor; }
    }
    
    public override ProtoDoubleRange DefaultInstanceForType {
      get { return global::ProtoDoubleRange.DefaultInstance; }
    }
    
    public override ProtoDoubleRange BuildPartial() {
      if (resultIsReadOnly) {
        return result;
      }
      resultIsReadOnly = true;
      return result.MakeReadOnly();
    }
    
    public override Builder MergeFrom(pb::IMessage other) {
      if (other is ProtoDoubleRange) {
        return MergeFrom((ProtoDoubleRange) other);
      } else {
        base.MergeFrom(other);
        return this;
      }
    }
    
    public override Builder MergeFrom(ProtoDoubleRange other) {
      if (other == global::ProtoDoubleRange.DefaultInstance) return this;
      PrepareBuilder();
      if (other.HasFrom) {
        From = other.From;
      }
      if (other.HasTo) {
        To = other.To;
      }
      this.MergeUnknownFields(other.UnknownFields);
      return this;
    }
    
    public override Builder MergeFrom(pb::ICodedInputStream input) {
      return MergeFrom(input, pb::ExtensionRegistry.Empty);
    }
    
    public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      PrepareBuilder();
      pb::UnknownFieldSet.Builder unknownFields = null;
      uint tag;
      string field_name;
      while (input.ReadTag(out tag, out field_name)) {
        if(tag == 0 && field_name != null) {
          int field_ordinal = global::System.Array.BinarySearch(_protoDoubleRangeFieldNames, field_name, global::System.StringComparer.Ordinal);
          if(field_ordinal >= 0)
            tag = _protoDoubleRangeFieldTags[field_ordinal];
          else {
            if (unknownFields == null) {
              unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
            }
            ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
            continue;
          }
        }
        switch (tag) {
          case 0: {
            throw pb::InvalidProtocolBufferException.InvalidTag();
          }
          default: {
            if (pb::WireFormat.IsEndGroupTag(tag)) {
              if (unknownFields != null) {
                this.UnknownFields = unknownFields.Build();
              }
              return this;
            }
            if (unknownFields == null) {
              unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
            }
            ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
            break;
          }
          case 9: {
            result.hasFrom = input.ReadDouble(ref result.from_);
            break;
          }
          case 17: {
            result.hasTo = input.ReadDouble(ref result.to_);
            break;
          }
        }
      }
      
      if (unknownFields != null) {
        this.UnknownFields = unknownFields.Build();
      }
      return this;
    }
    
    
    public bool HasFrom {
      get { return result.hasFrom; }
    }
    public double From {
      get { return result.From; }
      set { SetFrom(value); }
    }
    public Builder SetFrom(double value) {
      PrepareBuilder();
      result.hasFrom = true;
      result.from_ = value;
      return this;
    }
    public Builder ClearFrom() {
      PrepareBuilder();
      result.hasFrom = false;
      result.from_ = 0D;
      return this;
    }
    
    public bool HasTo {
      get { return result.hasTo; }
    }
    public double To {
      get { return result.To; }
      set { SetTo(value); }
    }
    public Builder SetTo(double value) {
      PrepareBuilder();
      result.hasTo = true;
      result.to_ = value;
      return this;
    }
    public Builder ClearTo() {
      PrepareBuilder();
      result.hasTo = false;
      result.to_ = 0D;
      return this;
    }
  }
  static ProtoDoubleRange() {
    object.ReferenceEquals(global::CommonModelMessages.Descriptor, null);
  }
}

#endregion


#endregion Designer generated code

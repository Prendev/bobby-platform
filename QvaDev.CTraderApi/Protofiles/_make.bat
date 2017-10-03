@rem protoc.exe --proto_path=.\ --cpp_out=.\ .\CommonModelMessages.proto
@rem protoc.exe --proto_path=.\ --java_out=.\ .\CommonModelMessages.proto
@rem protoc.exe --proto_path=.\ --python_out=.\ .\CommonModelMessages.proto
google\ProtoGen\bin\NET35\Debug\protogen --protoc_dir=google -namespace=cTraderApi CommonMessages.proto CommonModelMessages.proto OpenApiMessages.proto OpenApiModelMessages.proto

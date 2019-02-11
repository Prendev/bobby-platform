using System;

namespace TradeSystem.IlyaFastFeedIntegration
{
	public class FastFeedCircularBuffer
	{
		public delegate void OnMessageDelegate(byte[] message);

		public const int BufferSize = 8192;
		public byte[] Buffer;
		private int _currentPtr;
		public int BufferEndPointer = 0;
		private int _msgStart = 0;
		private int _msgEnd = -1;

		public OnMessageDelegate OnMessage;

		//0: reading msg & waiting for =
		//1: next char should be #
		//2: next char should be =
		private int _state = 0;

		public FastFeedCircularBuffer()
		{
			Buffer = new byte[BufferSize];
		}

		public void OnRead(int count)
		{
			BufferEndPointer += count;
			for (; _currentPtr < BufferEndPointer; _currentPtr++)
			{
				switch (_state)
				{
					case 0:
						if (Buffer[_currentPtr] == '=')
						{
							_state = 1;
						}

						break;
					case 1:
						_state = Buffer[_currentPtr] == '#' ? 2 : 0;
						break;
					case 2:
						if (Buffer[_currentPtr] == '=')
						{
							_msgEnd = _currentPtr;
							MessageDone();
							_msgEnd = -1;
							_msgStart = _currentPtr + 1;
							if (_msgStart == BufferSize)
							{
								_msgStart = 0;
							}

							_state = 0;
						}
						else
						{
							_state = 0;
						}

						break;
				}

				if (BufferSize != BufferEndPointer) continue;
				BufferEndPointer = 0;
				_currentPtr = 0;
			}
		}

		private void MessageDone()
		{
			if (_msgEnd < _msgStart)
			{
				var bufferEndLength = BufferSize - _msgStart;
				var msg = new byte[bufferEndLength + _msgEnd];
				Array.Copy(Buffer, _msgStart, msg, 0, bufferEndLength);
				Array.Copy(Buffer, 0, msg, bufferEndLength, _msgEnd);
				OnMessage(msg);
			}
			else
			{
				var l = _msgEnd - _msgStart;
				var msg = new byte[l];
				Array.Copy(Buffer, _msgStart, msg, 0, l);
				OnMessage(msg);
			}

		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipsSignageDisplaySicp
{
    public class SicpMessage
    {
        /// <summary>
        /// The monitor ID to send/recieve messages from. (a.k.a monitor ID). Signal mode: Display Address range from 1 to 255. Broadcast mode: Display Address is 0 which indicates no ACK or Report is expected.
        /// </summary>
        /// <value>Positive byte value</value>
        public byte MonitorId { get; set; }

        /// <summary>
        /// The group ID to send/recieve messages from.
        /// </summary>
        /// <value>Positive byte value</value>
        public byte? GroupId { get; set; }

        /// <summary>
        /// The payload data that the message contains.
        /// </summary>
        /// <value></value>
        public byte[] Data { get; set; }

        /// <summary>
        /// Defines the format for SICP messages.
        /// </summary>
        /// <param name="monitorId">Monitor ID. Signal mode: Display Address range from 1 to 255. Broadcast mode: Display Address is 0 which indicates no ACK or Report is expected.</param>
        /// <param name="groupId"></param>
        /// <param name="data"></param>
        public SicpMessage(byte monitorId, byte? groupId = null, byte[] data = null)
        {
            if (monitorId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monitorId), "Monitor ID cannot be lower than 0. Choose 1-255 for signal mode and 0 for broadcast mode.");
            }

            if (groupId != null && groupId.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID cannot be lower than 0. Choose 0 for broadcast, 1-254 for groups and 255 for \"off\".");
            }

            if (data != null && data.Length > 36)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Data payload is too large. Needs to maximum 36 bytes.");
            }

            this.MonitorId = monitorId;
            this.GroupId = groupId;
            this.Data = data;
        }

        public byte[] ToArray()
        {
            var buffer = new List<byte>();
            buffer.Add(this.MonitorId);

            if (this.GroupId != null)
            {
                buffer.Add(this.GroupId.Value);
            }

            if (this.Data != null)
            {
                buffer.AddRange(this.Data);
            }

            var messageSize = (byte)(buffer.Count + 2); // 2 = (message size) + (checksum)
            buffer.Insert(0, messageSize);

            var checksum = CalculateChecksum(buffer);
            buffer.Add(checksum);

            return buffer.ToArray();
        }


        /// <summary>
        /// Parse an array of bytes into a SicpMessage.
        /// </summary>
        /// <param name="buffer">The byte array with buffer data</param>
        /// <param name="bytesCount">Number of bytes to read from the buffer</param>
        /// <returns>A new instance of SicpMessage</returns>
        public static SicpMessage Parse(byte[] buffer, int bytesCount)
        {
            var checksum = buffer[bytesCount - 1];
            var correctChecksum = CalculateChecksum(buffer.Take(bytesCount - 1));

            if (checksum != correctChecksum)
            {
                throw new Exception(string.Format("Invalid checksum in provided buffer. Received {0:X2}. Expected {1:X2}.", checksum, correctChecksum));
            }

            var messageSize = buffer[0];
            var monitor = buffer[1];
            var group = buffer[2];

            var dataLength = bytesCount - 4;
            byte[] data = new byte[dataLength];
            Buffer.BlockCopy(buffer, 3, data, 0, dataLength);

            return new SicpMessage(monitor, group, data);
        }

        protected static byte CalculateChecksum(IEnumerable<byte> buffer)
        {
            byte checksum = 0;

            foreach (var bit in buffer)
            {
                checksum ^= bit;
            }

            return checksum;
        }
    }
}



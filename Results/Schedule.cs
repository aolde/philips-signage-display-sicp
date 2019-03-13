using System;
using System.Collections;
using PhilipsSignageDisplaySicp.Models;

namespace PhilipsSignageDisplaySicp.Results
{
    public class Schedule : ISicpResult, ISicpCommandParameters
    {
        private const int TIMESPAN_NULL_VALUE = 24;

        public bool Enabled { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public VideoSource VideoSource { get; set; }
        public Playlist Playlist { get; set; }
        public WorkingDays WorkingDays { get; set; }

        public void Parse(byte[] parameters)
        {
            Enabled = parameters[0].ToBool();
            StartTime = parameters[1] == TIMESPAN_NULL_VALUE ? TimeSpan.FromSeconds(0) : new TimeSpan(parameters[1], parameters[2], 0);
            EndTime = parameters[3] == TIMESPAN_NULL_VALUE ? TimeSpan.FromSeconds(0) : new TimeSpan(parameters[3], parameters[4], 0);
            VideoSource = (VideoSource)parameters[5];
            WorkingDays = (WorkingDays)parameters[6];
            Playlist = (Playlist)parameters[7];
        }

        public byte[] ToBytes()
        {
            return new byte[8] {
                Enabled.ToByte(),
                (byte)StartTime.Hours,
                (byte)StartTime.Minutes,
                (byte)EndTime.Hours,
                (byte)EndTime.Minutes,
                (byte)VideoSource,
                (byte)WorkingDays,
                (byte)Playlist
            };
        }
    }

}
namespace PhilipsSignageDisplaySicp.Models
{
    public enum InputSource : byte
    {
        None = 0x00,
        Video = 0x01,
        SVideo = 0x02,
        Component = 0x03,
        VGA = 0x05,
        USB1 = 0x0C,
        USB2 = 0x08,
        HDMI1 = 0x0D,
        HDMI2 = 0x06,
        HDMI3 = 0x0F,
        DisplayPort1 = 0x0A,
        DisplayPort2 = 0x07,
        DVI_D = 0x0E,
        CardDVI_D = 0x09,
        CVI2 = 0x04,
        CardOPS = 0x0B,
        Browser = 0x10,
        SmartCMS = 0x11,
        DigitalMediaServer = 0x12,
        InternalStorage = 0x13,
        // Reserved = 0x14,
        // Reserved = 0x15,
        MediaPlayer = 0x16,
        PDFPlayer = 0x17,
        Custom = 0x18,
    }
}
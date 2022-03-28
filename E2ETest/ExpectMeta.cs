namespace HeadElement.E2ETest
{
    internal static class ExpectMeta
    {
        // "name|property|http-equiv|media|content"

        public static readonly string[] AtHome = new[]{
                "||||",
                "meta-N0||||value-N0-A",
                "meta-N2||||value-N2-A",
                "meta-N3||||value-N3-A",
                "meta-N5|||media-X|value-N5-A",
                "viewport||||width=device-width",
                "|meta-P0|||value-P0-A",
                "|meta-P2|||value-P2-A",
                "|meta-P3|||value-P3-A",
                "||meta-H0||value-H0-A",
                "||meta-H2||value-H2-A",
                "||meta-H3||value-H3-A"            };
        public static readonly string[] AtCounter = new[]{
                "||||",
                "meta-N0||||value-N0-A",
                "meta-N1||||value-N1-B",
                "meta-N2||||value-N2-B",
                "meta-N5|||media-X|value-N5-B",
                "meta-N5|||media-Y|value-N5-C",
                "viewport||||width=device-width",
                "|meta-P0|||value-P0-A",
                "|meta-P1|||value-P1-B",
                "|meta-P2|||value-P2-B",
                "||meta-H0||value-H0-A",
                "||meta-H1||value-H1-B",
                "||meta-H2||value-H2-B"
            };
        public static readonly string[] AtFetchData = new[]{
                "||||",
                "meta-N0||||value-N0-A",
                "meta-N1||||value-N1-C",
                "meta-N3||||value-N3-A",
                "meta-N4||||value-N4-C",
                "meta-N5|||media-X|value-N5-A",
                "meta-N5|||media-Y|value-N5-C",
                "viewport||||width=device-width",
                "|meta-P0|||value-P0-A",
                "|meta-P1|||value-P1-C",
                "|meta-P3|||value-P3-A",
                "|meta-P4|||value-P4-C",
                "||meta-H0||value-H0-A",
                "||meta-H1||value-H1-C",
                "||meta-H3||value-H3-A",
                "||meta-H4||value-H4-C"
            };
        public static readonly string[] AtOnAfterRender = new[]{
                "||||",
                "keywords||||2nd keywords",
                "meta-N0||||value-N0-A",
                "meta-N2||||value-N2-A",
                "meta-N5|||media-X|value-N5-A",
                "viewport||||width=device-width",
                "|meta-P0|||value-P0-A",
                "|meta-P3|||value-P3-A",
                "|og:title|||2nd title",
                "||meta-H2||value-H2-2nd",
                "||meta-H3||value-H3-A"
            };
        public static readonly string[] AtOnAfterRenderPrerendered = new[]{
                "||||",
                "keywords||||1st keywords",
                "meta-N0||||value-N0-A",
                "meta-N2||||value-N2-A",
                "meta-N3||||value-N3-A",
                "meta-N5|||media-X|value-N5-A",
                "viewport||||width=device-width",
                "|meta-P0|||value-P0-A",
                "|meta-P2|||value-P2-A",
                "|meta-P3|||value-P3-A",
                "|og:title|||1st title",
                "||meta-H0||value-H0-A",
                "||meta-H2||value-H2-1st",
                "||meta-H3||value-H3-A"
            };
    }
}

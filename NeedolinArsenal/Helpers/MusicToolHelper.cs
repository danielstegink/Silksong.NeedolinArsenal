using System.Collections.Generic;
using UnityEngine;

namespace NeedolinArsenal.Helpers
{
    /// <summary>
    /// Identifier for the 4 tools that can upgrade the needolin
    /// </summary>
    public enum MusicTool
    {
        Voltvessel,
        Lifeblood,
        Lantern,
        PinBadge
    }

    public static class MusicToolHelper
    {
        /// <summary>
        /// Tracks which tool has been chosen 
        /// </summary>
        public static MusicTool? chosenTool;

        /// <summary>
        /// Metal music clip; played when Voltvessel chosen
        /// </summary>
        public static AudioClip? metalClip;

        /// <summary>
        /// Country music clip; played when Wispfire Lantern chosen
        /// </summary>
        public static AudioClip? countryClip;

        /// <summary>
        /// Jazz music clip; played when Plasmium Phial chosen
        /// </summary>
        public static AudioClip? jazzClip;

        /// <summary>
        /// Shimasen music clip; played when Pin Badge chosen
        /// </summary>
        public static AudioClip? shimasenClip;

        /// <summary>
        /// Gets a list of all music-altering tools equipped
        /// </summary>
        /// <param name="equippedTools"></param>
        /// <returns></returns>
        public static List<MusicTool> GetMusicTools(List<string> equippedTools)
        {
            List<MusicTool> musicTools = new List<MusicTool>();
            if (equippedTools.Contains("Lightning Rod"))
            {
                musicTools.Add(MusicTool.Voltvessel);
            }

            if (equippedTools.Contains("Wisp Lantern"))
            {
                musicTools.Add(MusicTool.Lantern);
            }

            if (equippedTools.Contains("Lifeblood Syringe"))
            {
                musicTools.Add(MusicTool.Lifeblood);
            }

            if (equippedTools.Contains("Pinstress Tool"))
            {
                musicTools.Add(MusicTool.PinBadge);
            }

            return musicTools;
        }

        /// <summary>
        /// Gets the audio clip associated with the chosen tool
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static AudioClip? GetAudioClip(MusicTool? tool)
        {
            return tool switch
            {
                MusicTool.Voltvessel => metalClip,
                MusicTool.Lantern => countryClip,
                MusicTool.Lifeblood => jazzClip,
                _ => shimasenClip
            };
        }
    }
}

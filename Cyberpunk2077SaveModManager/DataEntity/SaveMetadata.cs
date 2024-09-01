using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cyberpunk2077SaveModManager.DataEntity.Save
{
    public record Position(double X, double Y, double Z);

    public record Metadata
    {
        public string gameDefsetion { get; set; }
        public string activeQuests { get; set; }
        public string trackedQuestEntry { get; set; }
        public string trackedQuest { get; set; }
        public string mainQuest { get; set; }
        public string debugString { get; set; }
        public string locationName { get; set; }
        public Position playerPosition { get; set; }
        public double playTime { get; set; }
        public double playthroughTime { get; set; }
        public int nextSavableEntityID { get; set; }
        public int nextNonSavableEntityID { get; set; }
        public string lifePath { get; set; }
        public string bodyGender { get; set; }
        public string brainGender { get; set; }

        public double level { get; set; }
        [JsonIgnore]
        public string levelString => $"Level {(int)this.level}";

        public double streetCred { get; set; }
        public double gunslinger { get; set; }
        public double assault { get; set; }
        public double demolition { get; set; }
        public double athletics { get; set; }
        public double brawling { get; set; }
        public double coldBlood { get; set; }
        public double stealth { get; set; }
        public double engineering { get; set; }
        public double crafting { get; set; }
        public double hacking { get; set; }
        public double combatHacking { get; set; }
        public double strength { get; set; }
        public double intelligence { get; set; }
        public double reflexes { get; set; }
        public double technicalAbility { get; set; }
        public double cool { get; set; }
        public string setialBuildID { get; set; }
        public string finishedQuests { get; set; }
        public string playthroughID { get; set; }
        public string pointOfNoReturnId { get; set; }
        public string visitID { get; set; }
        public string buildSKU { get; set; }
        public string buildPatch { get; set; }
        public string difficulty { get; set; }
        public IList<string> facts { get; set; }
        public int saveVersion { get; set; }
        public int gameVersion { get; set; }

        public string timestampString { get; set; }
        [JsonIgnore]
        public string timestampIso => this.timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        [JsonIgnore]
        public DateTime timestamp => DateTime.ParseExact(this.timestampString, "HH:mm:ss, d.MM.yyyy", CultureInfo.InvariantCulture);

        public string name { get; set; }
        public string userName { get; set; }
        public string buildID { get; set; }
        public string platform { get; set; }
        public string censorFlags { get; set; }
        public string buildConfiguration { get; set; }
        public string fileSize { get; set; }
        public string isForced { get; set; }
        public string isCheckpoint { get; set; }
        public int setialLoadingScreenID { get; set; }
        public string isStoryMode { get; set; }
        public string isPointOfNoReturn { get; set; }
        public string isEndGameSave { get; set; }
        public string isModded { get; set; }
        public IList<string> additionalContentIds { get; set; }
    }

    public record Data
    {
        public Metadata metadata { get; set; }
    }

    public record SaveMetadata
    {
        public string RootType { get; set; }
        public Data Data { get; set; }
    }
}

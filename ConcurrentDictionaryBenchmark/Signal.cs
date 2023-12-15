using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using static BenchmarkDotNet.Engines.Engine;

namespace ConcurrentDictionaryBenchmark
{
    public sealed class Signal 
    {
        /// <inheritdoc />
        public DateTimeOffset StartTime { get; set; }

        /// <inheritdoc />
        public DateTimeOffset EndTime { get; set; }

        /// <inheritdoc />
        public SignalType SignalType { get; set; }

        /// <inheritdoc />
        public DateTimeOffset CreationTime { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public long? DurationInSeconds { get; set; }

        /// <inheritdoc />
        public string Locale { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public string CustomPropertiesAsString
        {
            get
            {
                return JsonSerializer.Serialize(CustomPropertiesJson);
            }

            set
            {
                CustomPropertiesJson = JsonSerializer.Deserialize<object>(value);
            }
        }

        /// <inheritdoc />
        public string CV { get; set; }

        /// <inheritdoc />
        public string ImmutableId { get; set; }

        /// <summary>
        /// CustomProperties from JSON
        /// </summary>
        [JsonPropertyName("CustomProperties")]
        public object CustomPropertiesJson { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SignalType
    {
        // Always keep this as the fist value in the Enum. All the types are to be added after this one.
        InvalidSignalType = 0,

        InstantMessage = 0x01,
        CallRecordSummarized = 0x02,
        MarkMessageAsRead = 0x03,
        MessageSent = 0x04,
        ViewMessage = 0x5,
        Reply = 0x6,
        ReplyAll = 0x7,
        Forward = 0x8,
        ConsumptionHorizonUpdated = 0x9,
        ActOnInsights = 0xa,
        FileAccessed = 0xb,
        ViewInsightDetails = 0xc,
        AcceptCalendarItem = 0x0d,
        AddedToSharedWithMe = 0x0e,
        CancelCalendarItem = 0x0f,
        ChangedProfile = 0x10,
        ClientPageActivitySignaled = 0x11,
        CollabResourcesAdded = 0x12,
        CommentCreated = 0x13,
        ContactCreated = 0x14,
        ContactUpdated = 0x15,
        CreateCalendarItem = 0x16,
        CreateGroupChat = 0x17,
        CreateSuggestedTask = 0x18,
        CreateTask = 0x19,
        DeclineCalendarItem = 0x1a,
        DeleteCalendarItem = 0x1b,
        DeleteSuggestedTask = 0x1c,
        DocumentViewed = 0x1d,
        FileDownloaded = 0x1e,
        FileModified = 0x1f,
        FileRenamed = 0x20,
        FileUploaded = 0x21,
        Flag = 0x22,
        FlagComplete = 0x23,
        ForwardCalendarItem = 0x24,
        IntentTaskDetected = 0x25,
        ItemShared = 0x26,
        ItemSharedUpdate = 0x27,
        ItemUnshared = 0x28,
        LinkClicked = 0x29,
        ListItemCreated = 0x2a,
        ListViewed = 0x2b,
        MarkAsUnread = 0x2c,
        MediaPlayback = 0x2d,
        MessageReaction = 0x2e,
        MicrosoftFeedInteracted = 0x2f,
        OfficeCanvasTaskDelete = 0x30,
        OfficeCanvasTaskUpdate = 0x31,
        OfficePromptResponse = 0x32,
        OfficeWorkspaceOpened = 0x33,
        OfficeWorkspacePageAdded = 0x34,
        OfficeWorkspacePageLinkAdded = 0x35,
        OneNotePageChanged = 0x36,
        OneNoteSectionModified = 0x37,
        PageViewed = 0x38,
        PostChannelMessage = 0x39,
        PreviewedAttachment = 0x3a,
        ProfileViewed = 0x3b,
        ReactedWithEmoji = 0x3c,
        ReceiveCalendarItem = 0x3d,
        RecommendationResultsInteraction = 0x3e,
        ReplyChannelMessage = 0x3f,
        SearchQuery = 0x40,
        SearchRecommendation = 0x41,
        SearchResultsInteraction = 0x42,
        SearchSuggestion = 0x43,
        SendMessageToUnifiedGroup = 0x44,
        ShareNotificationRequested = 0x45,
        SuggestionResultsInteraction = 0x46,
        TentativeAcceptedCalendarItem = 0x47,
        ThreadViewed = 0x48,
        TopicViewed = 0x49,
        TranscriptDeleted = 0x4a,
        UpdateCalendarItem = 0x4b,
        UpdateSuggestedTask = 0x4c,
        UserAtMentioned = 0x4d,
        UserPrimeTimePrediction = 0x4e,
        Viewed = 0x4f,
        ViewedSearchResults = 0x50,
        ViewedSearchResultsInteraction = 0x51,
        ViewTeamChannel = 0x52,
        VisitTeamChannel = 0x53,

        InvalidMaxValue
    }
}

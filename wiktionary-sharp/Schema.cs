using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JI = System.Text.Json.Serialization.JsonIgnoreAttribute;
using System.Text.Json;
using System.Text.Json.Serialization;
using MessagePack;
using RocksDbSharp;

namespace Wiktionary
{
    public class WordDefinitionListSerializer : ISpanDeserializer<List<WordDefinition>>
    {
        public static readonly WordDefinitionListSerializer Instance = new WordDefinitionListSerializer();
        private WordDefinitionListSerializer() 
        { 
        }

        public unsafe List<WordDefinition> Deserialize(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length == 0) return new();

            fixed (byte* pointer = &buffer.GetPinnableReference())
            {
                using var stream = new UnmanagedMemoryStream(pointer, buffer.Length);
                return MessagePackSerializer.Deserialize<List<WordDefinition>>(stream);
            }
        }

        internal static byte[] Serialize(List<WordDefinition> definitions)
        {
            return MessagePackSerializer.Serialize(definitions);
        }
    }

    [MessagePackObject]
    public class WordDefinition
    {
        [Key(0)]   [J("title")]                [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Title                   { get; set; }
        [Key(1)]   [J("redirect")]             [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Redirect                { get; set; }
        [Key(2)]   [J("senses")]               [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Sense[] Senses                 { get; set; }
        [Key(3)]   [J("pos")]                  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Pos                     { get; set; }
        [Key(4)]   [J("head_templates")]       [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Template[] HeadTemplates       { get; set; }
        [Key(5)]   [J("forms")]                [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Form[] Forms                   { get; set; }
        [Key(6)]   [J("word")]                 [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Word                    { get; set; }
        [Key(7)]   [J("lang")]                 [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Lang                    { get; set; }
        [Key(8)]   [J("lang_code")]            [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string LangCode                { get; set; }
        [Key(9)]   [J("etymology_text")]       [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string EtymologyText           { get; set; }
        [Key(10)]  [J("etymology_templates")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Template[] EtymologyTemplates  { get; set; }
        [Key(11)]  [J("related")]              [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Derived[] Related              { get; set; }
        [Key(12)]  [J("sounds")]               [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Sound[] Sounds                 { get; set; }
        [Key(13)]  [J("categories")]           [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Categories            { get; set; }
        [Key(14)]  [J("derived")]              [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Derived[] Derived              { get; set; }
        [Key(15)]  [J("synonyms")]             [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Derived[] Synonyms             { get; set; }
        [Key(16)]  [J("translations")]         [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Translation[] Translations     { get; set; }
        [Key(17)]  [J("inflection_templates")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Template[] InflectionTemplates { get; set; }
    }

    [MessagePackObject]
    public class Derived
    {
        [Key(0)]  [J("word")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Word { get; set; }
    }

    [MessagePackObject]
    public class Form
    {
        [Key(0)]  [J("form")]   [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string FormForm { get; set; }
        [Key(1)]  [J("tags")]   [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Tags   { get; set; }
        [Key(2)]  [J("source")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Source   { get; set; }
    }

    [MessagePackObject]
    public class Template
    {
        [Key(0)]  [J("name")]      [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Name                     { get; set; }
        [Key(1)]  [J("args")]      [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Dictionary<string, string> Args { get; set; }
        [Key(2)]  [J("expansion")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Expansion                { get; set; }
    }

    [MessagePackObject]
    public class Sense
    {
        [Key(0)]  [J("raw_glosses")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] RawGlosses { get; set; }
        [Key(1)]  [J("categories")]   [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Categories { get; set; }
        [Key(2)]  [J("tags")]         [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Tags       { get; set; }
        [Key(3)]  [J("glosses")]      [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Glosses    { get; set; }
        [Key(4)]  [J("alt_of")]       [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Derived[] AltOf     { get; set; }
        [Key(5)]  [J("wikipedia")]    [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Wikipedia  { get; set; }
        [Key(6)]  [J("topics")]       [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Topics     { get; set; }
        [Key(7)]  [J("synonyms")]     [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Derived[] Synonyms  { get; set; }
        [Key(8)]  [J("antonyms")]     [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Derived[] Antonyms  { get; set; }
        [Key(9)]  [J("examples")]     [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Example[] Examples  { get; set; }
        [Key(10)]  [J("form_of")]     [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public FormOf[] FormOf     { get; set; }
    }

    [MessagePackObject]
    public class Example
    {
        [Key(0)]  [J("text")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Text { get; set; }
        [Key(1)]  [J("ref")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Ref  { get; set; }
        [Key(2)]  [J("type")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Type { get; set; }
    }

    [MessagePackObject]
    public class FormOf
    {
        [Key(0)]  [J("word")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Word  { get; set; }
        [Key(1)]  [J("extra")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Extra { get; set; }
    }

    [MessagePackObject]
    public class Sound
    {
        [Key(0)]  [J("ipa")]     [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Ipa    { get; set; }
        [Key(1)]  [J("tags")]    [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Tags { get; set; }
        [Key(2)]  [J("audio")]   [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Audio  { get; set; }
        [Key(3)]  [J("text")]    [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Text   { get; set; }
        [Key(4)]  [J("ogg_url")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string OggUrl { get; set; }
        [Key(5)]  [J("mp3_url")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Mp3Url { get; set; }
        [Key(6)]  [J("rhymes")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Rhymes { get; set; }
    }

    [MessagePackObject]
    public class Translation
    {
        [Key(0)]  [J("lang")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Lang   { get; set; }
        [Key(1)]  [J("code")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Code   { get; set; }
        [Key(2)]  [J("sense")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Sense  { get; set; }
        [Key(3)]  [J("word")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Word   { get; set; }
        [Key(4)]  [J("roman")] [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string Roman  { get; set; }
        [Key(5)]  [J("tags")]  [JI(Condition = JsonIgnoreCondition.WhenWritingDefault)] public string[] Tags { get; set; }
    }
}

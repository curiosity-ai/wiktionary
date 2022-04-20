using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;
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
        [Key(0)]  [J("title")]                 public string Title                   { get; set; }
        [Key(1)]  [J("redirect")]              public string Redirect                { get; set; }
        [Key(2)]  [J("senses")]                public Sense[] Senses                 { get; set; }
        [Key(3)]  [J("pos")]                   public string Pos                     { get; set; }
        [Key(4)]  [J("head_templates")]        public Template[] HeadTemplates       { get; set; }
        [Key(5)]  [J("forms")]                 public Form[] Forms                   { get; set; }
        [Key(6)]  [J("word")]                  public string Word                    { get; set; }
        [Key(7)]  [J("lang")]                  public string Lang                    { get; set; }
        [Key(8)]  [J("lang_code")]             public string LangCode                { get; set; }
        [Key(9)]  [J("etymology_text")]        public string EtymologyText           { get; set; }
        [Key(10)]  [J("etymology_templates")]  public Template[] EtymologyTemplates  { get; set; }
        [Key(11)]  [J("related")]              public Derived[] Related              { get; set; }
        [Key(12)]  [J("sounds")]               public Sound[] Sounds                 { get; set; }
        [Key(13)]  [J("categories")]           public string[] Categories            { get; set; }
        [Key(14)]  [J("derived")]              public Derived[] Derived              { get; set; }
        [Key(15)]  [J("synonyms")]             public Derived[] Synonyms             { get; set; }
        [Key(16)]  [J("translations")]         public Translation[] Translations     { get; set; }
        [Key(17)]  [J("inflection_templates")] public Template[] InflectionTemplates { get; set; }
    }

    [MessagePackObject]
    public class Derived
    {
        [Key(0)]  [J("word")] public string Word { get; set; }
    }

    [MessagePackObject]
    public class Form
    {
        [Key(0)]  [J("form")]   public string FormForm { get; set; }
        [Key(1)]  [J("tags")]   public string[] Tags   { get; set; }
        [Key(2)]  [J("source")] public string Source   { get; set; }
    }

    [MessagePackObject]
    public class Template
    {
        [Key(0)]  [J("name")]      public string Name                     { get; set; }
        [Key(1)]  [J("args")]      public Dictionary<string, string> Args { get; set; }
        [Key(2)]  [J("expansion")] public string Expansion                { get; set; }
    }

    [MessagePackObject]
    public class Sense
    {
        [Key(0)]  [J("raw_glosses")]  public string[] RawGlosses { get; set; }
        [Key(1)]  [J("categories")]   public string[] Categories { get; set; }
        [Key(2)]  [J("tags")]         public string[] Tags       { get; set; }
        [Key(3)]  [J("glosses")]      public string[] Glosses    { get; set; }
        [Key(4)]  [J("alt_of")]       public Derived[] AltOf     { get; set; }
        [Key(5)]  [J("wikipedia")]    public string[] Wikipedia  { get; set; }
        [Key(6)]  [J("topics")]       public string[] Topics     { get; set; }
        [Key(7)]  [J("synonyms")]     public Derived[] Synonyms  { get; set; }
        [Key(8)]  [J("antonyms")]     public Derived[] Antonyms  { get; set; }
        [Key(9)]  [J("examples")]     public Example[] Examples  { get; set; }
        [Key(10)]  [J("form_of")]     public FormOf[] FormOf     { get; set; }
    }

    [MessagePackObject]
    public class Example
    {
        [Key(0)]  [J("text")] public string Text { get; set; }
        [Key(1)]  [J("ref")]  public string Ref  { get; set; }
        [Key(2)]  [J("type")] public string Type { get; set; }
    }

    [MessagePackObject]
    public class FormOf
    {
        [Key(0)]  [J("word")]  public string Word  { get; set; }
        [Key(1)]  [J("extra")] public string Extra { get; set; }
    }

    [MessagePackObject]
    public class Sound
    {
        [Key(0)]  [J("ipa")]     public string Ipa    { get; set; }
        [Key(1)]  [J("tags")]    public string[] Tags { get; set; }
        [Key(2)]  [J("audio")]   public string Audio  { get; set; }
        [Key(3)]  [J("text")]    public string Text   { get; set; }
        [Key(4)]  [J("ogg_url")] public string OggUrl { get; set; }
        [Key(5)]  [J("mp3_url")] public string Mp3Url { get; set; }
        [Key(6)]  [J("rhymes")]  public string Rhymes { get; set; }
    }

    [MessagePackObject]
    public class Translation
    {
        [Key(0)]  [J("lang")]  public string Lang   { get; set; }
        [Key(1)]  [J("code")]  public string Code   { get; set; }
        [Key(2)]  [J("sense")] public string Sense  { get; set; }
        [Key(3)]  [J("word")]  public string Word   { get; set; }
        [Key(4)]  [J("roman")] public string Roman  { get; set; }
        [Key(5)]  [J("tags")]  public string[] Tags { get; set; }
    }
}

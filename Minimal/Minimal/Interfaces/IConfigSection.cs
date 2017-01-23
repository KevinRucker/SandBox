using System.Collections.Specialized;

namespace Minimal.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConfigSection
    {
        /// <summary>
        /// 
        /// </summary>
        HybridDictionary SectionData { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string this[string index] { get; }
    }
}

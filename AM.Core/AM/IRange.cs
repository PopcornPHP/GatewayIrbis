/* IRange.cs -- generic range.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

namespace AM
{
    /// <summary>
    /// Generic range.
    /// </summary>
    internal interface IRange < T >
    {
        /// <summary>
        /// Low bound of the range.
        /// </summary>
        T LowBound { get; set; }

        /// <summary>
        /// High bound of the range.
        /// </summary>
        T HighBound { get; set; }
    }
}

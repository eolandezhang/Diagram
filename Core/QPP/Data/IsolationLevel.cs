
namespace QPP.Data
{
    /// <summary>
    /// 指定連接的交易鎖定行為。
    /// </summary>
    public enum IsolationLevel
    {
        /// <summary>
        /// 使用與指定不同的隔離等級，但無法判斷該等級。
        /// </summary>
        Unspecified = -1,
        /// <summary>
        /// 無法覆寫來自隔離程度更深之交易的暫止變更。
        /// </summary>
        Chaos = 16,
        /// <summary>
        /// 可以進行 Dirty 讀取，這表示未發出共用鎖定，並且沒有生效的獨佔鎖定。
        /// </summary>
        ReadUncommitted = 256,
        /// <summary>
        /// 當正在讀取資料來避免 Dirty 讀取時，會使用共用鎖定，但是在交易結束之前，資料可以變更，這將會產生非重複讀取或虛設資料。 
        /// </summary>  
        ReadCommitted = 4096,
        /// <summary>
        /// 鎖定是加諸於查詢中使用的所有資料，以防止其他使用者更新資料。防止非重複讀取，但是仍然可能造成虛設資料列。
        /// </summary>
        RepeatableRead = 65536,
        /// <summary>
        /// 範圍鎖定會置於 System.Data.DataSet 上，以免其他使用者在交易完成前將資料列更新或插入至資料集中。
        /// </summary>
        Serializable = 1048576,
        /// <summary>
        /// 在其他應用程式正在修改相同資料時，儲存應用程式可以讀取的資料版本，藉此減少封鎖。指出即使重新查詢，您也無法從某個交易看到在其他交易中所產生的變更。
        /// </summary>
        Snapshot = 16777216,
    }
}

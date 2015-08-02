using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp.ViewModel.App_Data
{
    public class ItemDataRepository
    {
        public static ItemDataRepository Default;
        static ItemDataRepository()
        {
            Default = new ItemDataRepository();
        }
        public RangeObservableCollection<ItemData> CreateData(int num)
        {
            RangeObservableCollection<ItemData> DataCollection = new RangeObservableCollection<ItemData>()
            {
                //new ItemData("0","","01253-PAWNS-GOLD","","Images/fix.png"),
                //new ItemData("1","0","BGM-109650-108","","Images/green.png"),
                //new ItemData("2","1","PL-PLASTIC-PP 3015A","","Images/green.png"),
                //new ItemData("3","1","PL-PIGMENT-7408C",""),
                //new ItemData("4","0","PL005BA16001300PE001",""),
                //new ItemData("5","0","BGM-10965C-101",""),
                //new ItemData("6","0","BGM-10965C-102",""),
                //new ItemData("7","0","BGM-10965C-102aaaaaaaaaaaaaaaaaaa",""),
                new ItemData("0","","0.0","Root　Item1","Images/fix.png"),
                new ItemData("1","0", "1.1", "-","Images/green.png"),
                new ItemData("2","0", "1.2", "-"),
                new ItemData("3","1", "2.1", "-"),
                //new ItemData("4","2", "2.2", "-"),
                //new ItemData("5","3", "3.1", "-"),
                //new ItemData("6","0", "1.3", "-"),
                //new ItemData("7","4", "3.2", "-"),
                //new ItemData("8","4", "3.3", "-")
 
                /**/
            };
            for (var i = 0; i < num; i++)
            {
                DataCollection.Add(new ItemData(Guid.NewGuid().ToString(), "0", "Item " + i, "Images/green.png"));
            }
            return DataCollection;
        }

        public ItemData AddNew(string pid)
        {
            if (pid == null) throw new ArgumentNullException("pid");
            return new ItemData(Guid.NewGuid().ToString(), pid, "Item", "");
        }

        public ItemData AddNew(string pid, double left, double top)
        {
            return new ItemData(Guid.NewGuid().ToString(), pid, "Item", "", left, top, "");
        }
        public List<ItemData> GetAllSubItemDatas(IList<ItemData> itemDatas, ItemData itemData)
        {
            var result = new List<ItemData>();
            var child = new List<ItemData>();
            var list = itemDatas.Where(x => x.ItemParentId == itemData.ItemId);
            foreach (var subItem in list.Where(subitem => !result.Contains(subitem)))
            {
                child.Add(subItem);
                result.Add(subItem);
                foreach (var item in child)
                {
                    result.AddRange(GetAllSubItemDatas(itemDatas, item));
                }
            }
            return result;
        }
    }
}

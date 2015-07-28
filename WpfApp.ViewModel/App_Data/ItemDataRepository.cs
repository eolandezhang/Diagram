using System;
using System.Collections.ObjectModel;

namespace WpfApp.ViewModel.App_Data
{
    public class ItemDataRepository
    {
        public static ItemDataRepository Default;
        public ObservableCollection<ItemData> DataCollection { get; set; }


        static ItemDataRepository()
        {
            Default = new ItemDataRepository();
        }

        public ItemDataRepository()
        {
            DataCollection = new ObservableCollection<ItemData>()
            {
                new ItemData("0","","01253-PAWNS-GOLD",""),
                new ItemData("1","0","BGM-109650-108",""),
                new ItemData("2","1","PL-PLASTIC-PP 3015A",""),
                new ItemData("3","1","PL-PIGMENT-7408C",""),
                new ItemData("4","0","PL005BA16001300PE001",""),
                new ItemData("5","0","BGM-10965C-101",""),
                new ItemData("6","0","BGM-10965C-102",""),
                new ItemData("7","0","BGM-10965C-102aaaaaaaaaaaaaaaaaaa",""),
                //new ItemData("0","","0.0","Root　Item1"),
                //new ItemData("1","0", "1.1", "-"),
                //new ItemData("2","0", "1.2", "-"),
                //new ItemData("3","1", "2.1", "-"),
                //new ItemData("4","2", "3.1", "-"),
                //new ItemData("5","3", "2.2", "-"),
                //new ItemData("6","0", "1.3", "-"),
                //new ItemData("7","4", "3.2", "-"),
 /*new ItemData(Guid.NewGuid().ToString(),"0", "Item 1", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 2", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 3", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 4", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 5", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 6", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 7", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 8", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 9", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 10", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 11", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 12", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 13", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 14", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 15", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 16", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 17", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 18", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 19", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 20", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 21", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 22", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 23", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 24", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 25", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 26", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 27", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 28", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 29", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 30", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 31", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 32", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 33", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 34", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 35", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 36", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 37", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 38", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 39", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 40", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 41", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 42", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 43", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 44", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 45", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 46", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 47", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 48", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 49", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 50", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 51", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 52", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 53", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 54", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 55", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 56", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 57", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 58", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 59", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 60", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 61", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 62", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 63", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 64", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 65", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 66", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 67", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 68", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 69", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 70", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 71", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 72", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 73", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 74", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 75", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 76", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 77", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 78", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 79", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 80", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 81", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 82", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 83", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 84", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 85", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 86", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 87", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 88", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 89", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 90", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 91", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 92", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 93", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 94", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 95", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 96", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 97", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 98", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 99", ""),
 new ItemData(Guid.NewGuid().ToString(),"0", "Item 100", ""),
 
                */
            };
        }

        public ItemData AddNew(string pid)
        {
            if (pid == null) throw new ArgumentNullException("pid");
            return new ItemData(Guid.NewGuid().ToString(), pid, "Item", "");
        }

        public ItemData AddNew(string pid, double left, double top)
        {
            return new ItemData(Guid.NewGuid().ToString(), pid, "Item", "", left, top);
        }
    }
}

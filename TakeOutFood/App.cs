namespace TakeOutFood
{
    using System;
    using System.Collections.Generic;

    public class App
    {
        private IItemRepository itemRepository;
        private ISalesPromotionRepository salesPromotionRepository;

        public App(IItemRepository itemRepository, ISalesPromotionRepository salesPromotionRepository)
        {
            this.itemRepository = itemRepository;
            this.salesPromotionRepository = salesPromotionRepository;
        }

        public string BestCharge(List<string> inputs)
        {
            List<string> itemIdList = new List<string>();
            List<int> itemNumberList = new List<int>();
            bool promotionExists = false;
            foreach (var element in inputs)
            {
                string[] splitResult = element.Split('x');
                itemIdList.Add(splitResult[0].Trim());
                itemNumberList.Add(int.Parse(splitResult[1].Trim()));
            }
            List<string> reductedItems = new List<string>();
            promotionExists = ReductionExists(itemIdList, reductedItems);
            
            string result = ReturnResult(promotionExists, reductedItems, itemIdList, itemNumberList);
            Console.Write(result);
            return result;

        }

        private bool ReductionExists(List<string> itemIdList, List<string> reductedItems)
        {
            List<SalesPromotion> promotionList = salesPromotionRepository.FindAll();
            List<string> promotionItemList = promotionList[0].RelatedItems;
            foreach (var element in itemIdList)
            {
                if (promotionItemList.Contains(element))
                {
                    reductedItems.Add(element);
                }
            }
            return reductedItems.Count != 0;
        }

        private string ReturnResult(bool promotionExists, List<string> reductedItems, List<string> itemIdList, List<int> itemNumberList)
        {
            string result = "============= Order details =============" + '\n';
            int i = 0;
            foreach (var element in itemIdList)
            {
                result += itemRepository.FindAll().Find(item => item.Id == element).Name + " x " + itemNumberList[i].ToString() + " = ";
                result +=
                    (itemRepository.FindAll().Find(item => item.Id == element).Price * itemNumberList[i]).ToString();
                result += " yuan"+'\n';
                i++;
            }

            i = 0;
            double reducedValue = 0;
            if (promotionExists)
            {
                result += "-----------------------------------" + '\n';
                result += "Promotion used:" + '\n';
                result += "Half price for certain dishes (";
                foreach (var element in reductedItems)
                {
                    if (i == 0)
                    {
                        result += itemRepository.FindAll().Find(item => item.Id == element).Name;
                    }
                    else
                    {
                        result += ", " + itemRepository.FindAll().Find(item => item.Id == element).Name;
                    }

                    reducedValue += (itemRepository.FindAll().Find(item => item.Id == element).Price * 
                                     itemNumberList[itemIdList.IndexOf(element)]) * 0.5;
                    i++;
                }
                result += "), ";
                result += "saving " + reducedValue.ToString() + " yuan" + '\n';
            }

            result += "-----------------------------------" + '\n';
            i = 0;
            double sumPrice = 0;
            foreach (var element in itemIdList)
            {
                sumPrice += itemRepository.FindAll().Find(item => item.Id == element).Price * itemNumberList[i];
                i++;
            }

            sumPrice -= reducedValue;
            result += "Total：" + sumPrice.ToString() + " yuan" + '\n';
            result += "===================================";
            return result;
        }



        //private string PromotionItem(List<string> itemIdList, List<int> itemNubmerList, List<string> reductedItems)
        //{
        //    string itemIdToChoose = "";
        //    double price = 0;
        //    foreach (var element in reductedItems)
        //    {
        //        double itemPrice = itemRepository.FindAll().Find(item => item.Id == element).Price;
        //        if (itemPrice * itemNubmerList[itemIdList.IndexOf(element)] > price)
        //        {
        //            itemIdToChoose = element;
        //        }
        //    }

        //    return itemIdToChoose;

        //}
    }
}

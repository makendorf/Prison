using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Network
{
    public class Global
    {
        public enum Shop
        {
            None = 0,
            Shop1,
            Shop2,
            Shop3,
            Shop4,
            Shop5,
            Shop6,
            Shop7,
            Shop8,
            Shop9,
            Shop10,
            Shop11,
            Shop12,
            Shop13,
            Shop14,
            Shop15,
            Shop16,
            Shop17,
            Shop18,
            Shop19,
            Shop20
        }
        public static string GetGUID(Shop shop)
        {
            switch (shop)
            {
                case Shop.Shop1:
                    return "3c36ce5f-c743-45ef-83e6-d7eb5d8a47e4";
                case Shop.Shop2:
                    return "f1492bb8-6304-499c-ae89-8726428fba19";
                case Shop.Shop3:
                    return "31a14af9-df04-4848-aada-50b3cf4e0c7b";
                case Shop.Shop4:
                    return "40a2256e-e3e3-4164-832d-707ff7908a93";
                case Shop.Shop5:
                    return "cd4cdbac-6eb5-45c0-9179-885646971fc0";
                case Shop.Shop6:
                    return "4596cd20-05f2-4096-8156-5e6357912f63";
                case Shop.Shop7:
                    return "afda5238-15ac-495b-9354-c62f21712e7f";
                case Shop.Shop8:
                    return "83fe2943-a4e0-4739-8ff9-0c3b4cf12340";
                case Shop.Shop9:
                    return "d6248db2-2fb9-4180-be46-d1d976a5324a";
                case Shop.Shop10:
                    return "e4ffefd8-ba8f-4c9c-b05f-bfaccff99d89";
                case Shop.Shop11:
                    return "303a2ebe-3417-4b6c-8c45-c6a87926035b";
                case Shop.Shop12:
                    return "74be1c8a-2f51-4806-a696-97acdaa41196";
                case Shop.Shop13:
                    return "310d8999-1c7c-49af-990a-76ed28a79db8";
                case Shop.Shop14:
                    return "65eda003-3eaa-4388-a780-82eee623d204";
                case Shop.Shop15:
                    return "ab05d6f3-3a07-405a-bfa9-671a8fa6e4f9";
                case Shop.Shop16:
                    return "e89746f2-7371-454a-8b52-2cf82cf69c2e";
                case Shop.Shop17:
                    return "1170b3a7-0f44-4ea6-8e43-3b872e6b5d56";
                case Shop.Shop18:
                    return "15a179f8-7197-474d-a460-6e542c1d18cc";
                case Shop.Shop19:
                    return "ac0fb039-aa25-4279-a578-e438d57118e1";
                case Shop.Shop20:
                    return "fdeec569-ae49-4cfe-a589-59f3be5e825e";
            }
            return String.Empty;
        }

        public const string ServerID = "0f8fad5b-d9cb-469f-a165-70867728950e";
    }
}

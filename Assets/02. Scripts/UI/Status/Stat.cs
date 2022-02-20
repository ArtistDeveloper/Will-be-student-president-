using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI.status
{
    public enum StatusKinds
    {
        Reputation,
        Eloquence,
        Friends,
        Money,
        None
    }

    public class Stat
    {
        public static StatusKinds DistinguishStatkinds(string parentName)
        {    
            StatusKinds statusKind = StatusKinds.None;

            switch (parentName)
            {
                case "Reputation":
                    statusKind = StatusKinds.Reputation;
                    break;
                case "Eloquence":
                    statusKind = StatusKinds.Eloquence;
                    break;
                case "Friends":
                    statusKind = StatusKinds.Friends;
                    break;
                case "Money":
                    statusKind = StatusKinds.Money;
                    break;
                default:
                    break;
            }

            return statusKind;
        }
    }
}
#define G_LOG

using ShipDock.Testers;
using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLTester : Singletons<KLTester>, ITester
    {
        public const int LOG0 = 0;

        public KLTester()
        {
            Tester.Instance.AddTester(this);
            Tester.Instance.AddLogger(this, LOG0, "{0}");
        }

        public string Name { get; set; }
    }
}

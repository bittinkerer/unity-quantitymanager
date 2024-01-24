using Assets.Scripts.Play.Handlers;
using Packages.Estenis.EventActions_;
using System;
using UnityEngine;
using Packages.Estenis.UnityExts_;

namespace Packages.com.esteny.quantitymanager.Runtime.Managers
{
    public class QuantityRecurringUpdaterAction : BaseGameObjectAction
    {
        [SerializeField] private int _change;
        [SerializeField] private float _changeEverySeconds;
        [SerializeField] private QuantityManager _quantityManager;

        private Coroutine _updaterCoroutine;

        protected override void Action(object data)
        {
            _updaterCoroutine = 
                this.RunCoroutineEvery(() => _quantityManager.ChangeCurrent(_change), _changeEverySeconds);
        }
    }
}

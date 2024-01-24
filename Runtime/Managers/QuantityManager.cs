using Assets.Scripts.Editors;
using Packages.Estenis.GameEvent_;
using UnityEngine;

namespace Assets.Scripts.Play.Handlers
{
    /// <summary>
    /// Keeps track of a min, a max and a current
    /// </summary>
    public class QuantityManager : EventMonoBehaviour
    {
        [SerializeField] protected int _min;
        [SerializeField] protected int _max;
        [SerializeField] protected int _terminalMax;
        [SerializeField] protected int _initial; 

        [SerializeField] private GameEventObject   _onCurrentMinEvent;
        [SerializeField] private GameEventObject   _onCurrentMaxEvent;
        [SerializeField] private GameEventObject   _onCurrentChangeEvent;
        [SerializeField] private GameEventObject   _onMaxChangeEvent;
        [SerializeField] private GameEventObject   _onInitEvent;

        [SerializeField][DisableInt] private int _current;

        private Coroutine _checkMinQuantityRoutine;

        public int Max
        {
            get => _max;
            protected set => _max = Mathf.Clamp(value, _min + 1, _terminalMax);
        }

        public int Initial => _initial;

        public int Min => _min - 1;

        /// <summary>
        /// 0-based index of current
        /// </summary>
        public int Current
        {
            get => _current;
            protected set => _current = Mathf.Clamp(value, Min, Max-1);
        }

        private void Awake()
        {
            Current = _initial; 
        }

        private void Start()
        {
            if (_onInitEvent != null)
            {
                _onInitEvent.Raise(EventId, this, this);
            }
            
        }

        private void OnDisable()
        {
            if(_checkMinQuantityRoutine != null)
            {
                _checkMinQuantityRoutine = null;
            }
        }

        public void ChangeMax(int amount)
        {
            if (amount == 0 || (Max >= _terminalMax && amount > 0) || (Max == 0 && amount < 0))
            {
                return;
            }

            if (Max + amount > _terminalMax)
            { 
                amount = _terminalMax - _max;
            }
            Max += amount;
            _onMaxChangeEvent.Raise(EventId, this, amount);
            ChangeCurrent(_max - Current - 1); // Fill to the new max and account for 0-based index of Current (thus the -1)
        }

        public void ChangeCurrent(int amount)
        {
            amount = Mathf.Clamp(amount, Min - Current, Max - Current - 1);
            // Current == Min -> Defeated, so no actions available after that.
            if (amount == 0 || (Current == Max && amount > 0) || (Current == Min && amount < 0))
            {
                return;
            }

            Current += amount;

            // NOTE: For events that cause transitions, only the first event will be handled and others discarded
            if (Current <= Min && _onCurrentMinEvent)
            {
                _onCurrentMinEvent.Raise(EventId, this, null);
            }

            if (LaunchMinQuantityEvent())
            {
                _onCurrentMaxEvent.Raise(EventId, this, null);
            }

            if (_onCurrentChangeEvent)
            {
                //Debug.Log($"Triggering QuantityChangeEvent with Current: {Current} and Change: {addend}");
                _onCurrentChangeEvent.Raise(EventId, this, amount);
            }
        }

        public void ResetQuantity()
        {
            Current = Initial;
        }

        public bool LaunchMinQuantityEvent() =>
            Current <= Min && _onCurrentMinEvent;
    }
}


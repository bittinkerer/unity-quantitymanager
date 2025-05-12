#if UNITY_EDITOR
using System;
using Assets.Scripts.Editors;
#endif
using Packages.Estenis.GameEvent_;
using UnityEngine;

namespace Assets.Scripts.Play.Handlers {
  public class QuantityLoadData {
    public int Max = Int32.MinValue;
    public int Min = Int32.MinValue;
  }

  /// <summary>
  /// Keeps track of a min, a max and a current
  /// </summary>
  public class QuantityManager : EventMonoBehaviour {
    [SerializeField] protected int _min;
    [SerializeField] protected int _max;
    [SerializeField] protected int _terminalMax;
    [SerializeField] protected int _initial;

    [SerializeField] private GameEventObject _onCurrentMinEvent;
    [SerializeField] private GameEventObject _onCurrentMaxEvent;
    [SerializeField] private GameEventObject _onCurrentChangeEvent;
    [SerializeField] private GameEventObject _onMaxChangeEvent;
    [SerializeField] private GameEventObject _onInitEvent;

    [SerializeField]
#if UNITY_EDITOR
    [DisableInt]
#endif
    private int _current;

    public int PreviousMax { get; private set; }
    public int PreviousMin { get; private set; }
    public int PreviousCurrent { get; private set; }

    public int Max {
      get => _max;
      protected set => _max = Mathf.Clamp(value, _min + 1, _terminalMax);
    }

    public int Initial => _initial;

    public int Min => _min;

    /// <summary>
    /// 0-based index of current
    /// </summary>
    public int Current {
      get => _current;
      protected set => _current = Mathf.Clamp(value, Min, Max);
    }

    protected void Awake() {
      
    }

    private void Start() {
      Current = _initial;
      if (_onInitEvent != null) {
        _onInitEvent.Raise(EventId, this.gameObject, this);
      }
    }

    public void ChangeMax(int amount) {
      PreviousMax = Max;
      Max += amount;
      if (PreviousMax != Max && _onMaxChangeEvent) {
        _onMaxChangeEvent.Raise(EventId, this.gameObject, amount);
      }
      else {
        Debug.LogWarning($"{this.name}.{nameof(QuantityManager)}: Call to change Max produced no change. Max: {Max}, Amount: {amount}");
      }
    }

    public void ChangeCurrent(int amount) {
      PreviousCurrent = Current;
      Current += amount;
      if (Current == PreviousCurrent) {
        Debug.LogWarning($"{this.name}.{nameof(QuantityManager)}: Call to change Current produced no change. Current: {Current}, Amount: {amount}");
        return;
      }

      // NOTE: For events that cause transitions, only the first event will be handled and others discarded
      if (Current <= Min && _onCurrentMinEvent) {
        _onCurrentMinEvent.Raise(EventId, this.gameObject, null);
      }

      if (Current >= Max && _onCurrentMaxEvent) {
        _onCurrentMaxEvent.Raise(EventId, this.gameObject, null);
      }

      if (_onCurrentChangeEvent) {
        _onCurrentChangeEvent.Raise(EventId, this.gameObject, amount);
      }
    }

    public void ResetQuantity() {
      Current = Initial;
    }

    public void LoadData(QuantityLoadData quantityLoadData) {
      if (quantityLoadData != null) {
        if (quantityLoadData.Max != Int32.MinValue) {
          int amount = _max - quantityLoadData.Max;
          _max = quantityLoadData.Max;
          _current = _initial = _max;
          _onMaxChangeEvent.Raise(EventId, this.gameObject, amount);
        }
        if (quantityLoadData.Min != Int32.MinValue) {
          _min = quantityLoadData.Min;
        }
      }
    }
  }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace UISystem {

    public class InGameUI : MenuUI
    {
        [SerializeField] MenuUI pauseMenuUI;

        List<Label> uiCounts;
        string LABEL_SUFFIX = "Count";

        Volume volume;


        #region MonoBehaviour Callbacks
        protected override void Awake()
        {
            base.Awake();
            EventManager.AddListener<InventoryChangedEvent>(SetCount);
            //Inventory.OnInventoryChanged += SetItem;
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<InventoryChangedEvent>(SetCount);
            //Inventory.OnInventoryChanged -= SetItem;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
        #endregion

        protected override void GrabVisualElements()
        {
	        base.GrabVisualElements();
            SetupItemCounts();
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            pauseMenuUI.OnOpen += () => pauseMenuUI.PreviousMenu = this;
        }

        void SetupItemCounts()
        {
            uiCounts = new List<Label>();

            foreach (var itemName in Enum.GetNames(typeof(ItemType)))
            {
                Label label = root.Q<Label>(itemName + LABEL_SUFFIX);
                if (label != null)
                    uiCounts.Add(label);
                else
                {
                    print("no label exists: " + itemName);
                }
            }
        }

        void SetCount(InventoryChangedEvent evt) => SetItem(evt.itemId, evt.itemCount);
        public void SetItem(int itemIndex, int count) => uiCounts[itemIndex].text = "" + count;
    }
}

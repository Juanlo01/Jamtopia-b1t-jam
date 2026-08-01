using UnityEngine;
using UnityEngine.UI;

namespace Yarn.Unity
{
    [RequireComponent(typeof(OptionItem))]
    // sibling of YarnSpinner::OptionItem by composition
    // handles state & style updating, regarding the addition of a background decorator box
    public class BoxedOptionItem : MonoBehaviour
    {
        [SerializeField] OptionItem optionItem;
        [SerializeField] Image boxImage;

        [SerializeField] Color inactiveColour = Color.white;
        [SerializeField] Color selectedColour = Color.white;
        [SerializeField] Color disabledColour = Color.grey;

        private enum BoxState { Inactive, Selected, Disabled }
        private BoxState? appliedState;

        // ensure reference to prerequisite sibling component (OptionItem)
        private void Reset()
        {
            optionItem = GetComponent<OptionItem>();
        }

        private void OnEnable()
        {
            appliedState = null;
        }

        // internal FSM for new box state received from prereq. sibling node, optionItem
        private void Update()
        {
            if (optionItem == null)
            {
                return;
            }

            BoxState state = !optionItem.IsInteractable()
                ? BoxState.Disabled
                : optionItem.IsHighlighted
                    ? BoxState.Selected
                    : BoxState.Inactive;

            if (state == appliedState)
            {
                return;
            }
            appliedState = state;

            switch (state)
            {
                case BoxState.Inactive:
                    ApplyInactiveStyle();
                    break;
                case BoxState.Selected:
                    ApplySelectedStyle();
                    break;
                case BoxState.Disabled:
                    ApplyDisabledStyle();
                    break;
            }
        }

        // internal styling handler: INACTIVE state
        protected virtual void ApplyInactiveStyle()
        {
            if (boxImage != null)
            {
                boxImage.color = inactiveColour;
            }
        }

        // internal styling handler: SELECTED state
        protected virtual void ApplySelectedStyle()
        {
            if (boxImage != null)
            {
                boxImage.color = selectedColour;
            }
        }

        // internal styling handler: DISABLED state
        protected virtual void ApplyDisabledStyle()
        {
            if (boxImage != null)
            {
                boxImage.color = disabledColour;
            }
        }
    }
}

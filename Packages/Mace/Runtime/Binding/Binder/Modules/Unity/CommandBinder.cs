using UnityEngine;
using UnityEngine.Events;

namespace Mace
{
    public class CommandBinder : ComponentBinder
    {
        [SerializeField] private BindingInfo command = BindingInfo.Command();
        private UnityEvent commandExecuteTrigger;

        protected override void Awake()
        {
            base.Awake();
            RegisterCommand(command).AddExecuteTrigger(commandExecuteTrigger);
        }

        public void ExecuteCommand()
        {
            commandExecuteTrigger?.Invoke();
        }
    }
}
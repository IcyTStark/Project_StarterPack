using FIO.ModularAddressableSystem;
using Sirenix.OdinInspector;
using TMS.Feedback;
using UnityEngine;
using VContainer;

public class TestScript : MonoBehaviour
{
    [Inject] private AddressablesManager addressablesManager;

    [Inject] private IFeedbackManager feedbackManager;

    [SerializeField] private AudioType audioType;

}

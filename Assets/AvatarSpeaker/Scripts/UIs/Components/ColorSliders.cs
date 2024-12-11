using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarSpeaker.UIs.Components
{
    public class ColorSliders : MonoBehaviour
    {
        [SerializeField] private Slider _redSlider;
        [SerializeField] private Slider _greenSlider;
        [SerializeField] private Slider _blueSlider;
        [SerializeField] private TMP_InputField _redInputField;
        [SerializeField] private TMP_InputField _greenInputField;
        [SerializeField] private TMP_InputField _blueInputField;

        /// <summary>
        /// 現在の色
        /// </summary>
        public readonly ReactiveProperty<Color> Current = new(Color.white);


        private void Start()
        {
            Current.Subscribe(v =>
            {
                _redSlider.value = Current.Value.r;
                _greenSlider.value = Current.Value.g;
                _blueSlider.value = Current.Value.b;
                _redInputField.text = $"{(int)(255 * Current.Value.r)}";
                _greenInputField.text = $"{(int)(255 * Current.Value.g)}";
                _blueInputField.text = $"{(int)(255 * Current.Value.b)}";
            });

            _redSlider.OnValueChangedAsAsyncEnumerable(destroyCancellationToken)
                .DistinctUntilChanged()
                .Subscribe(v => Current.Value = new Color(v, Current.Value.g, Current.Value.b));

            _greenSlider.OnValueChangedAsAsyncEnumerable(destroyCancellationToken)
                .DistinctUntilChanged()
                .Subscribe(v => Current.Value = new Color(Current.Value.r, v, Current.Value.b));

            _blueSlider.OnValueChangedAsAsyncEnumerable(destroyCancellationToken)
                .DistinctUntilChanged()
                .Subscribe(v => Current.Value = new Color(Current.Value.r, Current.Value.g, v));

            _redInputField.OnEndEditAsAsyncEnumerable(destroyCancellationToken)
                .DistinctUntilChanged()
                .Select(int.Parse)
                .Select(v => Mathf.Clamp01(v / 255f))
                .Subscribe(v => Current.Value = new Color(v, Current.Value.g, Current.Value.b));

            _greenInputField.OnEndEditAsAsyncEnumerable(destroyCancellationToken)
                .DistinctUntilChanged()
                .Select(int.Parse)
                .Select(v => Mathf.Clamp01(v / 255f))
                .Subscribe(v => Current.Value = new Color(Current.Value.r, v, Current.Value.b));

            _blueInputField.OnEndEditAsAsyncEnumerable(destroyCancellationToken)
                .DistinctUntilChanged()
                .Select(int.Parse)
                .Select(v => Mathf.Clamp01(v / 255f))
                .Subscribe(v => Current.Value = new Color(Current.Value.r, Current.Value.g, v));

            Current.RegisterTo(destroyCancellationToken);
        }
    }
}
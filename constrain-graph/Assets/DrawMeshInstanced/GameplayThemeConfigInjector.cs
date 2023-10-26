namespace GameLineup.Theme
{
    public class GameplayThemeConfigInjector : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] GameplayThemeConfig _data;
        [UnityEngine.SerializeField] Test_ThemeConfig _view;

        public void OnEnable()
        {
            _view.Setup(_data);
        }
    }
}

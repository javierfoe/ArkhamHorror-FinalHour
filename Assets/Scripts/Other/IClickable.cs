    using UnityEngine.Events;

    public interface IClickable<T>
    {
        UnityEvent<T> OnClick { get; }
        void OnMouseDown();
    }
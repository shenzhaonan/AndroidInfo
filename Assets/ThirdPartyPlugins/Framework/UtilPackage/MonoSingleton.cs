using UnityEngine;

namespace Framework.UtilPackage
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (null != instance) return instance;
                instance = FindObjectOfType<T>();
                if (null != instance) return instance;

                GameObject child = new GameObject(typeof(T).Name);
                child.transform.localPosition = Vector3.zero;
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                instance = child.AddComponent<T>();
                return instance;
            }
        }

        /// <summary>
        /// 如果重写Awake方法，请确保base.Awake()在其它代码之前执行
        /// 用于初始化单例
        /// 非必须，但此举可以节省一次FindObjectOfType调用
        /// </summary>
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (default(T) == instance)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                Debug.LogError("The class already has an instance. ");
                DestroyImmediate(this);
            }
        }

        /// <summary>
        /// 如果重写OnEnable方法，请确保base.OnEnable()在其它代码之前执行
        /// 用于重绑定单例
        /// 非必须，但是如果在OnDisable()中调用了父方法则必须在OnEnable()中进行重绑定
        /// </summary>
        protected virtual void OnEnable()
        {
            if (default(T) == instance)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                Debug.LogError("The class already has an instance. ");
                DestroyImmediate(this);
            }
        }

        /// <summary>
        /// 如果重写OnDisable方法，请确保base.OnDisable()在其它代码之后执行
        /// 用于销毁单例
        /// 非必须，但是基于Unity组件OnDisable()状态下某些方法不可用，如果需要在OnDisable()状态下调用该脚本的某些方法请进行剥离
        /// </summary>
        protected virtual void OnDisable()
        {
            instance = null;
        }
    }

    public class Singleton<T> where T : class, new()
    {
        private static T instance;

        public static T Instance
        {
            get { return instance ?? (instance = new T()); }
        }
    }
}
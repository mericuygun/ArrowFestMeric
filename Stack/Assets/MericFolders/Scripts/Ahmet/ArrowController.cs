using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Pool;

public class ArrowController : MonoBehaviour
{

    [Header("ArrowControl")]
    public GameObject arrowPrefab;
    public float distanceBetweenArrows;
    [SerializeField] private float startDistance = 10f;
    private ObjectPool<GameObject> arrowPool;
    private List<GameObject> activeArrowsList = new List<GameObject>();
    public int ArrowCount => activeArrowsList.Count;
    public static ArrowController Instance { get; private set; }
    public int baseArrowCount;
    public int arrowCount;


    [Header("Movement")]
    public float limitX;
    public static Kosu Current; // static deðiþkenler herhangi bir sýnýf tarafýndan eriþilebilir. 
    public float xSpeed;
    public float runningSpeed;
    private float _currentRunningSpeed;
    private float _lastTouchedX;
    public bool _finished;
    public float newX = 0;
    Animator anim;

    [Header("Other")]
    public TextMeshProUGUI arrowCountText;
    public GameObject textObject;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //   GameManager.Instance.onGameStartEvent.AddListener(OnGameStart);



        CreateArrowPool();
        SpawnArrow(baseArrowCount);
    }

    public void Start()
    {
        //
        anim = GetComponent<Animator>();
        arrowCount = baseArrowCount;
        PlayerPrefs.SetInt("avaible", 1);
        //
        PlayerPrefs.SetInt("OyunDevam", 1); // Sil baþka scripte yaz
        Application.targetFrameRate = 60;
    }

    public void changeScale()
    {
        if (Mathf.Abs(transform.position.x) > 0.4f)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1 - Mathf.Abs(Mathf.Abs(this.transform.position.x)-0.4f);
            transform.localScale = scale;
        }
    }

    public void changeDistanceBetweenArrows()
    {
        if (arrowCount < 30 && arrowCount > 0)
        {

            distanceBetweenArrows = 0.06f;
        }
        else if (arrowCount < 60 && arrowCount > 30)
        {
            distanceBetweenArrows = 0.05f;

        }
        else if (arrowCount < 90 && arrowCount > 60)
        {
            distanceBetweenArrows = 0.04f;

        }
        else if (arrowCount < 120 && arrowCount > 90)
        {
            distanceBetweenArrows = 0.035f;

        }
        else if (arrowCount > 120)
        {
            distanceBetweenArrows = 0.03f;

        }
    }
    private void Update()
    {
        textObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.04f, this.transform.position.z - 1f);

        arrowCountText.text = arrowCount.ToString();


        changeScale();
        changeDistanceBetweenArrows();

        if (this.transform.position.y <= -0.2f)
        {
            PlayerPrefs.SetInt("OyunDevam", 0);
            Time.timeScale = 0;
        }


        //   Debug.Log(stackParent.gameObject.GetComponent<StackController>()._stack.Count);


        float touchXDelta = 0;
        if (Input.touchCount > 0) // parmakla dokunma
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 5 * (Input.GetTouch(0).position.x - _lastTouchedX) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;
            }


        }
        else if (Input.GetMouseButton(0)) // mouse ile týklama
        {
            touchXDelta = Input.GetAxis("Mouse X");

        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);

        // karakter hareket ettirme kodu
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + runningSpeed * Time.deltaTime);


        transform.position = newPosition;

        //ArrowCounter(); __________
    }




    private void CreateArrowPool()
    {

        //   arrowPrefab = Resources.Load<GameObject>("Arrow");
        arrowPool = new ObjectPool<GameObject>(
            500,
            CreateFunction: () =>
            {
                GameObject _arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.Euler(90, 0, 0), transform);
                _arrow.SetActive(false);
                return _arrow;
            },
            OnPush: _arrow =>
            {
                _arrow.SetActive(false);
            },
            OnPop: _arrow =>
            {
                _arrow.transform.localPosition = Vector3.zero;
            }
        );
    }

    private void OnGameStart()
    {

    }



    public void DivideArrows(int divider)
    {
        if (divider < 1)
        {
            return;
        }

        float reduceAmount = ArrowCount * (divider - 1) / (float)divider;
        float remaining = ArrowCount - reduceAmount;

        if (remaining < 1)
        {
            GameManager.Instance.EndGame(false);
            return;
        }

        ReduceArrow(Mathf.CeilToInt(reduceAmount));
    }


    public void MultiplyArrows(int times)
    {
        if (times < 2)
        {
            return;
        }

        SpawnArrow(ArrowCount * (times - 1));
    }

    public void ReduceArrow(int amount)
    {
        if (amount >= ArrowCount)
        {
            GameManager.Instance.EndGame(false);
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject _arrow = activeArrowsList[0];
            activeArrowsList.Remove(_arrow);
            arrowPool.Push(_arrow);
        }

        ReorderArrows();
    }

    public void SpawnArrow(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject _arrow = arrowPool.Pop();
            _arrow.SetActive(true);
            activeArrowsList.Add(_arrow);
        }

        ReorderArrows();
    }

    private void ReorderArrows()
    {
        if (ArrowCount == 0)
        {
            return;
        }

        CapsuleCollider col = GetComponent<CapsuleCollider>();

        activeArrowsList[0].transform.localPosition = Vector3.zero;
        int arrowIndex = 1;
        int circleOrder = 1;

        while (true)
        {
            col.radius = 0.14f * circleOrder;
            float radius = circleOrder * distanceBetweenArrows;

            for (int i = 0; i < (circleOrder + 1) * 4; i++)
            {
                if (arrowIndex == ArrowCount)
                {
                    return;
                }

                float radians = 2 * Mathf.PI / (circleOrder + 1) / 4 * i;
                float vertical = Mathf.Sin(radians);
                float horizontal = Mathf.Cos(radians);

                Vector3 dir = new Vector3(horizontal, vertical, 0f);
                Vector3 newPosition = dir * radius;

                GameObject _arrow = activeArrowsList[arrowIndex];

                if (_arrow != null)
                {
                    _arrow.transform.DOKill();
                    _arrow.transform.DOLocalMove(newPosition, 0.25f);
                }

                arrowIndex++;
            }

            circleOrder++;
        }
    }

    IEnumerator wait01sec()
    {
        PlayerPrefs.SetInt("avaible", 0);
        yield return new WaitForSeconds(0.2f);
        PlayerPrefs.SetInt("avaible", 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("Gold"))
        {
         

            other.transform.DOScale(0f, 0.15f).OnComplete(() => other.gameObject.SetActive(false));
        }
        */
        //if (PlayerPrefs.GetInt("avaible")==1)
        //{
        if (other.tag == "times2")
        {

            MultiplyArrows(2);
            arrowCount = arrowCount * 2;
            StartCoroutine("wait01sec");
        }

        if (other.tag == "times4")
        {

            MultiplyArrows(4);
            arrowCount = arrowCount * 4;
            StartCoroutine("wait01sec");
        }


        if (other.tag == "per2")
        {

            DivideArrows(2);
            arrowCount = arrowCount / 2;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "per3") //hatalý
        {

            DivideArrows(3);
            arrowCount = arrowCount / 3;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "per10") //hatalý
        {

            DivideArrows(10);
            arrowCount = arrowCount / 10;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "plus10") //hatalý
        {

            SpawnArrow(10);
            arrowCount = arrowCount + 10;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "plus25") //hatalý
        {

            SpawnArrow(25);
            arrowCount = arrowCount + 25;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "plus50") //hatalý
        {

            SpawnArrow(50);
            arrowCount = arrowCount + 50;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "plus15") //hatalý
        {

            SpawnArrow(15);
            arrowCount = arrowCount + 15;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "plus65") //hatalý
        {

            SpawnArrow(65);
            arrowCount = arrowCount + 65;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "plus20") //hatalý
        {

            SpawnArrow(20);
            arrowCount = arrowCount + 20;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "plus40") //hatalý
        {

            SpawnArrow(40);
            arrowCount = arrowCount + 40;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "minus25") //hatalý
        {

            ReduceArrow(25);
            arrowCount = arrowCount - 25;
            StartCoroutine("wait01sec");
        }
        if (other.tag == "Enemy") 
        {

            ReduceArrow(3);
            arrowCount = arrowCount - 3;
            other.gameObject.GetComponent<Animator>().SetBool("Dead", true);
            other.gameObject.transform.GetChild(6).GetChild(0).GetChild(0).gameObject.SetActive(true);

            //GameObject.Find
            //StartCoroutine("wait01sec");
        }


        //}

    }
    void ArrowCounter()
    {
        GameObject.Find("ArrowCounter").transform.position = GameObject.Find("Arrow").transform.position;
        GameObject.Find("ArrowCounter").transform.LookAt(new Vector3(0, -1.5f, -2.5f));
    }
}

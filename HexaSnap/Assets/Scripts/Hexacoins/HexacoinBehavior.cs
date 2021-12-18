/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class HexacoinBehavior : MonoBehaviour {


    public static readonly float glitterShowingDuration = 100;


    private static Texture imageSmall;
    private static Texture imageMedium;
    private static Texture imageBig;

    private RawImage imageHexacoin;

    private bool isGlittering = false;
    private bool isGlitteringMore = false;

    private string coroutineTagGenerateGlitter;
    private string coroutineTagGenerateGlitterMore;


    void Awake() {

        coroutineTagGenerateGlitter = "generateGlitter_" + Constants.newRandomPositiveInt();
        coroutineTagGenerateGlitterMore = "generateGlitterMore_" + Constants.newRandomPositiveInt();

        if (imageSmall == null) {
            imageSmall = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_MENUS + "Hexacoin.SMALL");
            imageMedium = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_MENUS + "Hexacoin.MEDIUM");
            imageBig = GameHelper.Instance.loadTexture2DAsset(Constants.PATH_DESIGNS_MENUS + "Hexacoin.BIG");
        }

        imageHexacoin = GetComponent<RawImage>();
    }

    void Start() {

        startGlittering();
    }

    public void startGlittering() {
        
        if (isGlittering) {
            return;
        }

        isGlittering = true;

        Async.call(generateGlitters(), coroutineTagGenerateGlitter);
    }

    public void stopGlittering() {
        
        if (!isGlittering) {
            return;
        }

        setGlitteringMore(false);

        isGlittering = false;
        
        Async.cancel(coroutineTagGenerateGlitter);
        
        //remove current glitters
        GameObjectPoolBehavior pool = GameHelper.Instance.getPool();
        
        while (transform.childCount > 0) {
            
            GameObject goGlitter = transform.GetChild(0).gameObject;
            goGlitter.GetComponent<Animation>().Stop();

            pool.storeHexacoinGlitterGameObject(goGlitter);
        }
        
    }

    public void setGlitteringMore(bool isGlitteringMore) {
        
        if (isGlitteringMore == this.isGlitteringMore) {
            return;
        }
        
        this.isGlitteringMore = isGlitteringMore;
        
        if (isGlitteringMore) {
            
            Async.call(generateMoreGlitters(), coroutineTagGenerateGlitterMore);

        } else {

            Async.cancel(coroutineTagGenerateGlitterMore);
        }

    }

    private IEnumerator generateGlitters() {

        while (isGlittering) {

            generateRandomGlitter();
            
            float delaySec = Constants.newRandomFloat(0.7f, 1f);
            yield return new WaitForSeconds(delaySec);
        }

    }

    private IEnumerator generateMoreGlitters() {

        while (isGlitteringMore) {

            generateRandomGlitter();
            
            float delaySec = Constants.newRandomFloat(0.2f, 0.3f);
            yield return new WaitForSeconds(delaySec);
        }

    }

    private void generateRandomGlitter() {

        GameObjectPoolBehavior pool = GameHelper.Instance.getPool();

        int maxSize = 60;

        Vector3 randomPos = new Vector3(
            Constants.newRandomFloat(-maxSize, maxSize),
            Constants.newRandomFloat(-maxSize, maxSize), 
            0
        );

        GameObject goGlitter = pool.pickHexacoinGlitterGameObject(transform, true, randomPos);
        
        Async.call(glitterShowingDuration, () => {
            pool.storeHexacoinGlitterGameObject(goGlitter);
        });

    }

}

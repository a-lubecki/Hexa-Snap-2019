/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System.Collections;
using UnityEngine;


public class Activity4a : Activity4, DialogAnimationListener {

    public static readonly int POP_CODE_CHANGE_LEVEL = 8862;


    private bool hasCustomTitleLine;
    private bool mustOverlayGame;
    private int? previousLevel;
    private int nextLevel;
    private int? savedScore;

    private Animation animationBackground;

    private Transform trGoal;


    protected override MarkerBehavior getCurrentMarkerForInit(MarkerManager markerManager) {

        if (bundlePush != null && ((BundlePush4a) bundlePush).mustOverlayGame) {
            return markerManager.markerEPlay;
        }

        return base.getCurrentMarkerForInit(markerManager);
    }
    

    protected override string[] getPrefabNamesToLoad() {
		return new string[] { "Activity4" };
	}

	protected override string getTextTitle() {
		return Tr.get("Activity4.Text.Level");
	}

	protected override string getTextTarget() {
		return getTextTargetArcade(nextLevel);
	}

    protected override bool hasDefaultHexacoinWallet() {
        return true;
    }


    protected override void onCreate() {

		if (bundlePush == null) {

            previousLevel = 1;
            nextLevel = 1;

		} else {
			
			BundlePush4a b = (BundlePush4a) bundlePush;
            hasCustomTitleLine = b.hasCustomTitleLine;
            mustOverlayGame = b.mustOverlayGame;
            nextLevel = b.nextLevel;

            if (b.previousLevel.HasValue) {
                previousLevel = b.previousLevel.Value;
            } else {
                previousLevel = nextLevel;
            }

            if (b.savedScore.HasValue) {
                savedScore = b.savedScore.Value;
            }
		}

		base.onCreate();


        //put the wallet at a more visible position
        hexacoinsWalletBehavior.transform.position = Constants.newVector3(
            markerRef.posSafeAreaTopRight,
            -0.5f,
            -1,
            0
        );

        Transform parentTransform = markerRef.transform.Find("Activity4");

        if (hasCustomTitleLine) {

            Vector3 linePos = getTitleLinePos();

            //animating to right
            Line lineTitle = new Line(
                new Vector3(markerRef.posLeft.x - 1, linePos.y, linePos.z),
                linePos,
                SegmentThickness.LARGE,
                0,
                false
            );

            //create the missing title line (usually created with the push line)
            GameObject goTitle = GameHelper.Instance.getPool().pickLineGameObject(lineTitle);
            goTitle.transform.SetAsFirstSibling();
            lineDrawerTitle = new LineDrawer(lineTitle);
            GameHelper.Instance.getLineDrawersManager().register(lineDrawerTitle);
        }

        if (mustOverlayGame) {

            textPrevious.color = Constants.COLOR_TITLE_NEGATIVE;
            textNext.color = Constants.COLOR_TITLE_NEGATIVE;

            GameObject goBackground = GameHelper.Instance.getPool().pickDialogBackgroundGameObject(parentTransform);
            goBackground.GetComponent<DialogBehavior>().listener = this;
            animationBackground = goBackground.GetComponent<Animation>();

            //move to the back
            goBackground.transform.SetAsFirstSibling();
        }

        GoalCompletion goalCompletion = new GoalCompletion(nextLevel);

        trGoal = GameObject.Instantiate(
            GameHelper.Instance.loadPrefabAsset("Goal"),
            parentTransform).transform;
        trGoal.GetComponent<GoalCompletionBehavior>().init(goalCompletion);
        
        trGoal.gameObject.SetActive(false);

        hexacoinsWalletBehavior.setOnlyDisplayedOnChanges(true);
    }

    protected override void onResume(bool isFirst) {
        base.onResume(isFirst);

        if (mustOverlayGame) {

            Constants.playAnimation(animationBackground, null, false);

            textTitle.enabled = false;
        }

        bool isReversed = false;
        bool hasProgressed = false;

        string previous = "";
        string next = "";

        if (!previousLevel.HasValue) {

            next = nextLevel.ToString();

        } else {

            isReversed = (nextLevel < previousLevel);
            hasProgressed = (nextLevel > previousLevel);

            if (hasProgressed) {
                next = nextLevel.ToString();
                previous = previousLevel.Value.ToString();
            } else {
                next = previousLevel.Value.ToString();
                previous = nextLevel.ToString();
            }
        }

        Async.call(animateNextLevel(isReversed, hasProgressed, previous, next));
    }

    private IEnumerator animateNextLevel(bool isReversed, bool hasProgressed, string previous, string next) {

        if (mustOverlayGame) {

            yield return new WaitForSeconds(0.2f);

            //show a flying hexacoin then show the wallet
            if (hasProgressed) {

                yield return new WaitForSeconds(0.2f);

                Vector2 walletPos = hexacoinsWalletBehavior.GetComponent<RectTransform>().anchoredPosition;

                GameObjectPoolBehavior pool = GameHelper.Instance.getPool();
                GameObject go = pool.pickFlyingGameObject(
                    false,
                    getFirstLoadedGameObject().transform,
                    true,
                    new Vector3(walletPos.x + 20, walletPos.y - 400)
                );

                go.GetComponent<FlyingScoreBehavior>().startFlying("+1", () => {
                    pool.storeFlyingGameObject(false, go);
                });

                hexacoinsWalletBehavior.reveal();

                yield return new WaitForSeconds(0.5f);

                gameManager.addHexacoins(1, getActivityName(), T.Value.EARN_REASON_END_LEVEL);

                yield return new WaitForSeconds(0.5f);
            }
        }

        if (hasCustomTitleLine) {
            //draw the missing title line
            lineDrawerTitle.drawAnimated(0.3f, InterpolatorCurve.LINEAR, null);
            GameHelper.Instance.getAudioManager().playSound("Activity.Push");
        }

        if (mustOverlayGame) {

            textTitle.enabled = true;

            yield return new WaitForSeconds(0.2f);
        }

        //draw level changing
        triggerPreviousNext(
            isReversed, 
            previous,
            next,
            () => {

                if (!(previousActivity is Activity10)) {

                    //when onboarding, delay the push to have time to read the texts
                    var delayBeforePush = gameManager.hasPassedOnboarding ? 0 : 1;

                    Async.call(delayBeforePush, () => {

                        //push new game
                        BundlePushE10a b = new BundlePushE10a {
                            level = nextLevel
                        };

                        if (savedScore.HasValue) {
                            b.savedScore = savedScore.Value;
                        }

                        push(new Activity10a().setBundlePush(b));
                        pop();
                    });

                } else {

                    Async.call(0.5f, () => {

                        //return to the game after a delay
                        BundlePop4a b = new BundlePop4a {
                            nextLevel = nextLevel
                        };

                        pop(POP_CODE_CHANGE_LEVEL, b);
                    });
                }
        });

        //display goal
        yield return new WaitForSeconds(0.8f);

        trGoal.gameObject.SetActive(true);
            
        Constants.playAnimation(trGoal.GetComponent<Animation>(), null, false);
    }

    protected override void onDestroy() {
        base.onDestroy();

        if (mustOverlayGame) {
            
            Constants.playAnimation(animationBackground, null, true);
            
            //free created line
            GameHelper.Instance.getLineDrawersManager().unregister(lineDrawerTitle);
            GameHelper.Instance.getPool().storeLineGameObject(BaseModelBehavior.findModelBehavior<LineBehavior>(lineDrawerTitle.line));
            lineDrawerTitle = null;
        }

    }

    void DialogAnimationListener.onDialogAnimationStart(Animation anim, bool isReversed) {
        //do nothing
    }

    void DialogAnimationListener.onDialogAnimationFinish(Animation anim, bool isReversed) {

        if (isReversed) {

            GameHelper.Instance.getPool().storeDialogBackgroundGameObject(animationBackground.gameObject);

            //release
            animationBackground = null;
        }

    }

}

public struct BundlePush4a : BaseBundle {

    public bool hasCustomTitleLine;
    public bool mustOverlayGame;
    public int? previousLevel;
    public int nextLevel;
    public int? savedScore;

}

public struct BundlePop4a : BaseBundle {
    
    public int nextLevel;

}



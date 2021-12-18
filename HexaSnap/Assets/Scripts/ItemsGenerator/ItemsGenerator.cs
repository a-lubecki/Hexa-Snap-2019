/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemsGenerator : InGameModel {


    public static readonly int MAX_QUEUE_SIZE = 5;

    private static ItemsGeneratorListener to(BaseModelListener listener) {
        return (ItemsGeneratorListener)listener;
    }

    private static readonly Dictionary<ItemType, float> defaultGenerationProportions = new Dictionary<ItemType, float>() {
        { ItemType.Type1, 5 },
        { ItemType.Type5, 4 },
        { ItemType.Type20, 3 },
        { ItemType.Type100, 1 }
    };


    private readonly float graphMaxPercentage;
    private readonly int maxLevel;

    public bool isGeneratingItems { get; private set; }
    public bool isPaused { get; private set; }

    private bool canGenerateVoidItemBonus;
    private bool canGenerateVoidItemMalus;

    private float distanceLastGeneratedBonus;

	private List<Item> queue = new List<Item>();


	public ItemsGenerator(Activity10 activity, int maxLevel) : base(activity) {

        if (maxLevel <= 0) {
            throw new ArgumentException();
        }

        graphMaxPercentage = activity.graphPercentagesHolder.maxPercentage;
        this.maxLevel = maxLevel;
    }


    public void updateVoidBonusGeneration(bool canGenerateVoidItemBonus, bool canGenerateVoidItemMalus) {
        this.canGenerateVoidItemBonus = canGenerateVoidItemBonus;
        this.canGenerateVoidItemMalus = canGenerateVoidItemMalus;
    }

	private void fillQueue() {

		while (queue.Count < MAX_QUEUE_SIZE - 1) {
			enqueueItem(generateNewItem());
		}
	}

	private void emptyQueue() {

		List<Item> itemsToClear = new List<Item>(queue);
		queue.Clear();

		foreach (Item item in itemsToClear) {
			
			item.dequeue();
		}

		notifyListeners(listener => {
			to(listener).onItemsGeneratorClearItems(this);
		});

		foreach (Item item in itemsToClear) {
			
            item.destroy(ItemDestroyCause.System);
		}
	}

	private void enqueueItem(Item item) {

		//dequeue items to reach the max items number
		while (queue.Count >= MAX_QUEUE_SIZE) {
			dequeueItem();
		}

		queue.Add(item);
		item.enqueue();

		notifyListeners(listener => {
			to(listener).onItemsGeneratorAddItem(this, item);
		});
	}

	private void dequeueItem() {

		Item itemToRelease = queue[0];

		queue.RemoveAt(0);
		itemToRelease.dequeue();

		notifyListeners(listener => {
			to(listener).onItemsGeneratorDequeueItem(this, itemToRelease);
		});
	}

	public void pickItem(Item item) {

		if (item == null) {
			throw new ArgumentException();
		}
		if (!queue.Contains(item)) {
			throw new InvalidOperationException();
		}

		queue.Remove(item);
		item.dequeue();

		notifyListeners(listener => {
			to(listener).onItemsGeneratorRemoveItem(this, item);
		});

		fillQueue();

	}

	public int getQueueSize() {
		return queue.Count;
	}

	public Item getItemAt(int pos) {

		if (pos < 0) {
			throw new ArgumentException();
		}
		if (pos >= MAX_QUEUE_SIZE) {
			throw new ArgumentException();
		}

		return queue[pos];
	}

	public int getItemPos(Item item) {

		if (item == null) {
			throw new ArgumentException();
		}

		int pos = queue.IndexOf(item);

		if (pos < 0) {
			throw new InvalidOperationException("Item not found in queue : nb = " + queue.Count);
		}

		return pos;
	}

	public void startGeneration() {
        
		if (!GameHelper.Instance.getGameManager().isGamePlaying) {
			return;
		}

		if (isGeneratingItems) {
			return;
		}

        resumeGeneration();
        isGeneratingItems = true;

		fillQueue();

        Async.call(processItemsGeneration(), Constants.COROUTINE_TAG_ITEM_GENERATION);

		notifyListeners(listener => {
			to(listener).onItemsGeneratorStart(this);
		});
	}

    public void pauseGeneration() {
        
        isPaused = true;
    }

    public void resumeGeneration() {
        
        isPaused = false;
    }

    public void stopGeneration() {
        
		isGeneratingItems = false;

		Async.cancel(Constants.COROUTINE_TAG_ITEM_GENERATION);

		emptyQueue();

		notifyListeners(listener => {
			to(listener).onItemsGeneratorStop(this);
		});
	}

	public float getGenerationFrequenceSec() {

        var level = activity.getCurrentCappedLevel();

        if (activity is Activity10a && GameHelper.Instance.getGameManager().maxArcadeLevel <= 3) {

            //special cases for arcade onboarding
            if (level == 1) {
                return Constants.ONBOARDING_LEVEL_1_GENERATION_FREQUENCE_S;
            }
            if (level == 2) {
                return Constants.ONBOARDING_LEVEL_2_GENERATION_FREQUENCE_S;
            }
            if (level == 3) {
                return Constants.ONBOARDING_LEVEL_3_GENERATION_FREQUENCE_S;
            }
        }

        var percentage = 1 - (level / (float)maxLevel);

        //make the percentage exponential : x2
        percentage = percentage * percentage;

		var freq = Constants.MIN_GENERATION_FREQUENCE_S + percentage * (Constants.MAX_GENERATION_FREQUENCE_S - Constants.MIN_GENERATION_FREQUENCE_S);

        //more speed when the level is over the 20 levels
        level = activity.getCurrentLevel();

        if (level > Constants.MAX_LEVEL_ARCADE) {

            //capped at level 100
            if (level > Constants.MAX_LEVEL_HARDCORE) {
                level = Constants.MAX_LEVEL_HARDCORE;
            }

            //every 10 levels, the frequency increase
            freq -= Mathf.Floor((level - Constants.MAX_LEVEL_ARCADE) / 10f) * Constants.STEP_10_LEVELS_GENERATION_FREQUENCE_S;                
        }

        return freq;
	}

	private IEnumerator processItemsGeneration() {

		float durationSec = 0;
        bool wasPaused = false;

		while (isGeneratingItems) {

            var time = activity.timeManager.getTotalTimeScalePhysics();

            if (isPaused || time <= 0) {

                //reset to wait completely when resuming
                if (!wasPaused) {

                    wasPaused = true;

                    //improve the time between pause and the next generation (to have more fluid generation between item clic and next generation)
                    durationSec *= 0.2f;
                }

            } else {

                wasPaused = false;

                durationSec += Time.deltaTime * time / activity.bonusQueue.getEnqueuedGenerationFrequency();

                //if reaches the attempted duration, generate and wait for next item to be generated
                if (durationSec > getGenerationFrequenceSec()) {

                    durationSec = 0;

                    if (queue.Count >= MAX_QUEUE_SIZE - 1) {
                        dequeueItem();
                    }

                    enqueueItem(generateNewItem());
                }
            }

			yield return new WaitForSeconds(Constants.COROUTINE_FIXED_UPDATE_S);
		}

	}

    public bool canGenerateBonusOrMalus() {
        
        //the graph has some bonuses or the goal contains some void bonuses
        return ((getOnboardingGenerationPercentage() > 0 && graphMaxPercentage > 0)
                || canGenerateVoidItemBonus 
                || canGenerateVoidItemMalus);
    }

    private Item generateNewItem() {

        Item item = newItem();

        if (UnityEngine.Debug.isDebugBuild) {
            //item = newItemDebug();
        }

        activity.registerItem(item);

        return item;
    }

    private Item newItemDebug() {

        if (Constants.newRandomBool()) {
            return new ItemBonus(activity, GameHelper.Instance.getBonusManager().bonusTypeChoiceBonus);
        }

        return new ItemBonus(activity, GameHelper.Instance.getBonusManager().bonusTypeRowWipeout);
    }

    private float getOnboardingGenerationPercentage() {
        
        if (activity is Activity10a && 
            GameHelper.Instance.getGameManager().maxArcadeLevel <= Constants.LEVEL_WHEN_BONUS_AVAILABLE_FOR_ONBOARDING) {
            //there must be no bonus on the first levels of the first play
            return 0;
        }

        return 1;
    }

    private Dictionary<ItemType, float> getGenerationProportions() {
        
        //change for first levels in arcade mode
        if (activity is Activity10a) {
        
            var level = activity.getCurrentLevel();

            if (level == 1) {
                return new Dictionary<ItemType, float>() {
                    { ItemType.Type1, 12 },
                    { ItemType.Type5, 5 },
                    { ItemType.Type20, 5 },
                    { ItemType.Type100, 1 }
                };
            }

            if (level == 2) {
                return new Dictionary<ItemType, float>() {
                    { ItemType.Type1, 10 },
                    { ItemType.Type5, 10 },
                    { ItemType.Type20, 3 },
                    { ItemType.Type100, 1 }
                };
            }

            if (level == 3) {
                return new Dictionary<ItemType, float>() {
                    { ItemType.Type1, 10 },
                    { ItemType.Type5, 3 },
                    { ItemType.Type20, 8 },
                    { ItemType.Type100, 1 }
                };
            }
        }

        return defaultGenerationProportions;
    }

    private Item newItem() {

        //control the bonus generation to avoid having several bonuses chains
        float percentageItem = Constants.newRandomFloat(0, 1);

        if (graphMaxPercentage > 0) {
            
            //check if strictly less to not pass if percentageItem == 0 and graphMaxPercentage == 0
            if (percentageItem < Constants.MAX_PERCENTAGE_BONUS_GENERATION * graphMaxPercentage * getBonusItemControlPercentage() * getOnboardingGenerationPercentage()) {

                distanceLastGeneratedBonus = 0;

                //bonus generation :
                Item item = newItemBonus();
                if (item != null) {
                    return item;
                }

                //else : problem, the newType must not be null => fallback on default items
                UnityEngine.Debug.LogWarning("ItemsGenerator new bonus/malus item is null");
            }

        } else if ((canGenerateVoidItemBonus || canGenerateVoidItemMalus)
            && percentageItem <= Constants.MAX_PERCENTAGE_BONUS_GENERATION * getBonusItemControlPercentage()) {

            //check if can generate just a void bonus item
            distanceLastGeneratedBonus = 0;
            
            if (canGenerateVoidItemBonus && canGenerateVoidItemMalus) {
                //random 50/50 between bonus and malus
                return newVoidItem(Constants.newRandomBool());
            }

            return newVoidItem(canGenerateVoidItemMalus);
        }

        //else generate a default item with a random type
        percentageItem = Constants.newRandomFloat(0, 1);

        var proportions = new Dictionary<ItemType, float>(getGenerationProportions());

        //remove chances of generations for every types already in generator to avoid having the same generated types
        foreach (Item item in queue) {

            ItemType type = item.itemType;

            if (!proportions.ContainsKey(type)) {
                continue;
            }

            proportions[type] /= 1.5f;
        }

        float total = proportions.Values.Sum();
        float proportionsSum = 0;

        ItemType typeToGenerate = ItemType.Type1;

        //find wich item must be generated comparing the percentage and the types generation proportions
        foreach (KeyValuePair<ItemType, float> elem in proportions) {

            ItemType type = elem.Key;
            float p = elem.Value;

            proportionsSum += p;

            if (percentageItem <= proportionsSum / total) {
                //found
                typeToGenerate = type;
                break;
            }
        }

        distanceLastGeneratedBonus++;

        return new Item(activity, typeToGenerate);
	}

    private Item newItemBonus() {

        bool generateVoidBonus = false;
        bool generateVoidMalus = false;

        if (canGenerateVoidItemBonus && !activity.graphPercentagesHolder.hasAnyBonus()) {
            generateVoidBonus = true;
        }
        if (canGenerateVoidItemMalus && !activity.graphPercentagesHolder.hasAnyMalus()) {
            generateVoidMalus = true;
        }

        if (generateVoidBonus) {

            if (generateVoidMalus) {
                //void bonus or void malus
                return newVoidItem(Constants.newRandomBool());
            }

            //void bonus or default bonus or default malus
            if (Constants.newRandomBool()) {
                return newVoidItem(false);
            }

        } else if (generateVoidMalus) {
            //void malus or default bonus or default malus
            if (Constants.newRandomBool()) {
                return newVoidItem(true);
            }
        }

        //generate less malus than bonus in proportion when the player starts playing
        float malusProportion = 1;

        float levelProgression = (activity.getCurrentCappedLevel() - 1) / (float)activity.getMaxLevel();
        if (levelProgression <= 0.25) {
            malusProportion = 4 * levelProgression;
        }

        //generate a bonus (can be null)
        BonusType newType = activity.graphPercentagesHolder.getRandomBonusType(1, malusProportion);

        if (newType == null) {
            return null;
        }

        return new ItemBonus(activity, newType);
    }

    private Item newVoidItem(bool isMalus) {

        return new ItemBonus(activity, new BonusType(isMalus));
    }

    private float getBonusItemControlPercentage() {

        if (distanceLastGeneratedBonus <= 2) {
            //disable bonus generation when the item before was also a bonus
            return 0;
        }

        if (distanceLastGeneratedBonus <= 3) {
            return 0.5f;
        }
        
        if (distanceLastGeneratedBonus <= 4) {
            return 0.75f;
        }

        return 1;
    }

    public bool hasItemBonus() {

        foreach (Item item in queue) {

            if (item.itemType == ItemType.Bonus) {
                return true;
            }
        } 

        return false;
    }

}


/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public class Prop {

    //app opening
    public static readonly PropertyDateTime currentStart = new PropertyDateTime("currentStart");
    public static readonly PropertyDateTime lastStart = new PropertyDateTime("lastStart");
    public static readonly PropertyDateTime lastSync = new PropertyDateTime("lastSync");

    //user id
    public static readonly PropertyString userId = new PropertyString("userId");

    //tracked events
    public static readonly PropertyInt nbPlayArcade = new PropertyInt("nbPlayArcade"); //a_nb_play
    public static readonly PropertyInt nbPlayTimeAttack = new PropertyInt("nbPlayTimeAttack"); //t_nb_play
    public static readonly PropertyInt maxZoneUnlockedArcade = new PropertyInt("maxZoneUnlockedArcade"); //a_zone_unlocked
    public static readonly PropertyInt maxZoneUnlockedTimeAttack = new PropertyInt("maxZoneUnlockedTimeAttack"); //t_zone_unlocked
    public static readonly PropertyInt nbMaxHexacoins = new PropertyInt("nbMaxHexacoins"); //nb_hexacoins_max
    public static readonly PropertyInt totalEarnedHexacoins = new PropertyInt("totalEarnedHexacoins"); //nb_hexacoins_max
    public static readonly PropertyInt totalPaidHexacoins = new PropertyInt("totalPaidHexacoins"); //nb_hexacoins_max
    public static readonly PropertyInt nbPurchasedIAP = new PropertyInt("nbPaidIAP"); //iap_nb_paid
    public static readonly PropertyInt maxHexacoinsEarnedWithIAP = new PropertyInt("maxHexacoinsEarnedWithIAP"); //hexacoins_total_earned
    public static readonly PropertyInt totalHexacoinsEarnedWithIAP = new PropertyInt("totalHexacoinsEarnedWithIAP"); //hexacoins_total_paid
    public static readonly PropertyInt nbShareArcade = new PropertyInt("nbShareArcade"); //a_nb_share
    public static readonly PropertyInt nbShareTimeAttack = new PropertyInt("nbShareTimeAttack"); //t_nb_share
    public static readonly PropertyInt totalAppOpen = new PropertyInt("nbAppOpen"); //nb_app_open
    public static readonly PropertyInt totalTimeSecInArcade = new PropertyInt("totalTimeSecInArcade"); //a_total_time
    public static readonly PropertyInt totalTimeSecInTimeAttack = new PropertyInt("totalTimeSecInTimeAttack"); //t_total_time

    //share
    public static readonly PropertyString shareUrl = new PropertyString("shareUrl");
    public static readonly PropertyString referrerForShareUrl = new PropertyString("referrerForShareUrl");

}

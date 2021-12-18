/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */


public static class T {

    /**
     * All names are generated here :
     * https://docs.google.com/spreadsheets/d/138r4ilOdhOPcOvjIhSLI3SwfM9wLv2Fg3cjxlsg7ktk/edit#gid=2133627458
     */

    //tracking event names
    public static class Event {
        
        public static readonly string ACTIVITY = "activity";
        public static readonly string ONBOARDING_START = "onboarding_start";
        public static readonly string ONBOARDING_SKIP = "onboarding_skip";
        public static readonly string ONBOARDING_END = "onboarding_end";
        public static readonly string A_START = "a_start";
        public static readonly string A_LEVEL_END = "a_level_end";
        public static readonly string A_END_1 = "a_end_1";
        public static readonly string A_END_20 = "a_end_20";
        public static readonly string A_END_100 = "a_end_100";
        public static readonly string A_GAMEOVER = "a_gameover";
        public static readonly string A_SHARE_SCORE = "a_share_score";
        public static readonly string T_START = "t_start";
        public static readonly string T_GAMEOVER = "t_gameover";
        public static readonly string T_SHARE_SCORE = "t_share_score";
        public static readonly string OPEN_FROM_LINK = "open_from_link";
        public static readonly string A_NODE_UNLOCK = "a_node_unlock";
        public static readonly string A_NODE_ACTIVATE = "a_node_activate";
        public static readonly string A_NODE_DEACTIVATE = "a_node_deactivate";
        public static readonly string A_SLOTS_UNLOCK = "a_slots_unlock";
        public static readonly string A_SLOTS_ACTIVATE = "a_slots_activate";
        public static readonly string A_SLOTS_DEACTIVATE = "a_slots_deactivate";
        public static readonly string T_NODE_UNLOCK = "t_node_unlock";
        public static readonly string T_NODE_ACTIVATE = "t_node_activate";
        public static readonly string T_NODE_DEACTIVATE = "t_node_deactivate";
        public static readonly string T_SLOTS_UNLOCK = "t_slots_unlock";
        public static readonly string T_SLOTS_ACTIVATE = "t_slots_activate";
        public static readonly string T_SLOTS_DEACTIVATE = "t_slots_deactivate";
        public static readonly string OPTION_SOUNDS = "option_sounds";
        public static readonly string OPTION_MUSIC = "option_music";
        public static readonly string OPTION_CONTROLS = "option_controls";
        public static readonly string OPTION_CHARACTER = "option_character";
        public static readonly string LOGIN_START = "login_start";
        public static readonly string LOGIN_ACCEPT = "login_accept";
        public static readonly string LOGIN_CANCEL = "login_cancel";
        public static readonly string SIGN_IN = "sign_in";
        public static readonly string SIGN_UP = "sign_up";
        public static readonly string LOGOUT = "logout";
        public static readonly string DELETE_ACCOUNT = "delete_account";
        public static readonly string HEXACOIN_EARN = "hexacoin_earn";
        public static readonly string HEXACOIN_PAY_START = "hexacoin_pay_start";
        public static readonly string HEXACOIN_PAY_END = "hexacoin_pay_end";
        public static readonly string REWARD_CLICK = "reward_click";
        public static readonly string REWARD_COLLECTED = "reward_collected";
        public static readonly string IAP_CLICK = "iap_click";
        public static readonly string IAP_FAILED = "iap_failed";
        public static readonly string IAP_PAID = "iap_paid";
        public static readonly string ITEM_GENERATION = "item_generation";
        public static readonly string ITEM_SNAP = "item_snap";
        public static readonly string ITEM_UNSNAP = "item_unsnap";
        public static readonly string ITEM_STACK = "item_stack";
        public static readonly string ITEM_UNSTACK = "item_unstack";
        public static readonly string ITEM_SELECT = "item_select";
        public static readonly string STACK_INCREASE = "stack_increase";
        public static readonly string STACK_DECREASE = "stack_decrease";
        public static readonly string SHOW_POPUP_LEGAL = "show_popup_legal";
        public static readonly string REDIRECT_TERMS = "redirect_terms";
        public static readonly string REDIRECT_PRIVACY = "redirect_privacy";
        public static readonly string REDIRECT_LEGAL = "redirect_legal";
        public static readonly string SHOW_POPUP_ABOUT = "show_popup_about";
        public static readonly string REDIRECT_5STARS = "redirect_5stars";
        public static readonly string REDIRECT_SHARE = "redirect_share";
        public static readonly string REDIRECT_FACEBOOK = "redirect_facebook";
        public static readonly string REDIRECT_MORE = "redirect_more";
    }

    //user properties
    public static class Property {
        
        public static readonly string REFERRER = "referrer";
        public static readonly string A_NB_PLAY = "a_nb_play";
        public static readonly string T_NB_PLAY = "t_nb_play";
        public static readonly string A_MAX_SCORE = "a_max_score";
        public static readonly string A_MAX_LEVEL = "a_max_level";
        public static readonly string T_MAX_SCORE = "t_max_score";
        public static readonly string T_MAX_TIME = "t_max_time";
        public static readonly string A_MAX_ZONE = "a_max_zone";
        public static readonly string A_ZONE1 = "a_zone1";
        public static readonly string A_ZONE2 = "a_zone2";
        public static readonly string A_ZONE3 = "a_zone3";
        public static readonly string A_ZONE4 = "a_zone4";
        public static readonly string A_ZONE5 = "a_zone5";
        public static readonly string A_ZONE6 = "a_zone6";
        public static readonly string T_MAX_ZONE = "t_max_zone";
        public static readonly string T_ZONE1 = "t_zone1";
        public static readonly string T_ZONE2 = "t_zone2";
        public static readonly string T_ZONE3 = "t_zone3";
        public static readonly string T_ZONE4 = "t_zone4";
        public static readonly string HEXACOINS_NB = "hexacoins_nb";
        public static readonly string HEXACOINS_MAX = "hexacoins_max";
        public static readonly string HEXACOINS_TOTAL_EARNED = "hexacoins_total_earned";
        public static readonly string HEXACOINS_TOTAL_PAID = "hexacoins_total_paid";
        public static readonly string ADS = "ads";
        public static readonly string OPTION_SOUNDS = "option_sounds";
        public static readonly string OPTION_MUSIC = "option_music";
        public static readonly string OPTION_CONTROLS = "option_controls";
        public static readonly string IAP_NB_PURCHASED = "iap_nb_purchased";
        public static readonly string IAP_MAX_HEXACOINS = "iap_max_hexacoins";
        public static readonly string IAP_TOTAL_HEXACOINS = "iap_total_hexacoins";
        public static readonly string A_NB_SHARE = "a_nb_share";
        public static readonly string T_NB_SHARE = "t_nb_share";
        public static readonly string TOTAL_OPEN = "total_open";
        public static readonly string A_TOTAL_TIME = "a_total_time";
        public static readonly string T_TOTAL_TIME = "t_total_time";
    }

    //param names of events
    public static class Param {
        
        public static readonly string CURRENT = "current";
        public static readonly string PREVIOUS = "previous";
        public static readonly string LEVEL_START = "level_start";
        public static readonly string LEVEL = "level";
        public static readonly string LEVEL_NEXT = "level_next";
        public static readonly string LEVEL_TIME_SEC = "level_time_sec";
        public static readonly string SCORE = "score";
        public static readonly string TIME_SEC = "time_sec";
        public static readonly string REASON = "reason";
        public static readonly string REFERRER = "referrer";
        public static readonly string TAG = "tag";
        public static readonly string PERCENTAGE = "percentage";
        public static readonly string TYPE = "type";
        public static readonly string ORIGIN = "origin";
        public static readonly string NB_HEXACOINS = "nb_hexacoins";
        public static readonly string ID = "id";
        public static readonly string REMOVING_ADS = "removing_ads";
        public static readonly string ITEM_TYPE = "item_type";
        public static readonly string BONUS_TYPE = "bonus_type";
        public static readonly string NB_SNAPPED_ITEMS = "nb_snapped_items";
        public static readonly string NB_GROUPPED_ITEMS = "nb_groupped_items";
        public static readonly string STACK_SIZE = "stack_size";
        public static readonly string NB_STACKED_ITEMS = "nb_stacked_items";
    }

    //enum values for events and user properties
    public static class Value {

        public static readonly string TRUE = "true";
        public static readonly string FALSE = "false";
        public static readonly string ACTIVATED = "activated";
        public static readonly string DEACTIVATED = "deactivated";
        public static readonly string EARN_REASON_BLACK_ITEMS = "black_items";
        public static readonly string EARN_REASON_SHAKE_DEVICE = "shake_device";
        public static readonly string EARN_REASON_END_LEVEL = "end_level";
        public static readonly string EARN_REASON_END_GAME = "end_game";
        public static readonly string EARN_REASON_REWARD_DAILY = "reward_daily";
        public static readonly string EARN_REASON_REWARD_VIDEO = "reward_video";
        public static readonly string EARN_REASON_IAP = "iap";
        public static readonly string PAY_REASON_UNLOCK_ZONE = "unlock_zone";
        public static readonly string PAY_REASON_UNLOCK_SLOT = "unlock_slot";
        public static readonly string PAY_REASON_BOOST_LEVEL = "boost_level";
        public static readonly string LOGIN_FACEBOOK = "facebook";
        public static readonly string GAME_OVER_REASON_GIVE_UP = "give_up";
        public static readonly string GAME_OVER_REASON_OUSIDE_LIMIT = "outside_limit";
        public static readonly string GAME_OVER_REASON_END = "end";
        public static readonly string ZONE_ACTIVATED = "activated";
        public static readonly string ZONE_DEACTIVATED = "deactivated";
        public static readonly string ZONE_LOCKED = "locked";
        public static readonly string IAP_REASON_INVALID_RECEIPT = "invalid_receipt";
        public static readonly string IAP_REASON_NO_RECEIPT = "no_receipt";
        public static readonly string IAP_REASON_SENDING_FAILED = "sending_failed";
        public static readonly string CONTROLS_DRAG_HORIZONTAL = "drag_horizontal";
        public static readonly string CONTROLS_DRAG_AROUND_AXIS = "drag_around_axis";
    }

}


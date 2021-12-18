/**
 * Hexa Snap
 * Copyright © 2017 Aurélien Lubecki
 * All Rights Reserved
 */

#import "NativeUtils.h"

@implementation NativeUtils

+ (UIViewController*) getMainViewController {
    return UIApplication.sharedApplication.keyWindow.rootViewController;
}

+ (void) presentModalViewController:(UIViewController*)vc {

    UIViewController* rootVc = NativeUtils.getMainViewController;
    UIPopoverPresentationController* controller = vc.popoverPresentationController;
    controller.sourceView = rootVc.view;

    CGSize screenSize = UIScreen.mainScreen.bounds.size;
    controller.sourceRect = CGRectMake(0.5f * screenSize.width, screenSize.height, 0, 0);

    controller.permittedArrowDirections = UIPopoverArrowDirectionAny;

    [rootVc presentViewController:vc animated:YES completion:nil];
}

+ (NSString*) getAppOrigin {

    //obfuscated
}

+ (NSString*) getAppSignatures {

    //obfuscated
}

+ (NSString*) getNSString:(const char*)string {

    if (string == NULL) {
        return @"";
    }

    return [NSString stringWithUTF8String: string];
}

+ (NSArray*) getNSArrayOfNSString:(const char**)array count:(int)count  {

    if (array == NULL) {
        return nil;
    }

    if (count <= 0) {
        return NSArray.array;
    }

    NSMutableArray* res = [[NSMutableArray alloc] initWithCapacity:(count)];

    for (int i = 0 ; i < count ; i++) {
        [res addObject:[NativeUtils getNSString:array[i]]];
    }

    return res;
}

+ (const char*) getString:(NSString*)nsString {

    if (nsString == nil || nsString == (id)[NSNull null]) {
        return "";
    }

    return [nsString UTF8String];
}

@end

extern "C" {

    const char* call_getAppOrigin() {

        return [NativeUtils getString:[NativeUtils getAppOrigin]];
    }

    const char* call_getAppSignatures() {

        return [NativeUtils getString:[NativeUtils getAppSignatures]];
    }

}

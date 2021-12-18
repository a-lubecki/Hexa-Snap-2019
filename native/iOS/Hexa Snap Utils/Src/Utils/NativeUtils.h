/**
 * Hexa Snap
 * Copyright © 2017 Aurélien Lubecki
 * All Rights Reserved
 */

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface NativeUtils : NSObject

+ (UIViewController*) getMainViewController;

+ (void) presentModalViewController:(UIViewController*)vc;

+ (NSString*) getNSString:(const char*)string;

+ (NSArray*) getNSArrayOfNSString:(const char**)array count:(int)count;

@end

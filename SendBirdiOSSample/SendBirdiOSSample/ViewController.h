//
//  ViewController.h
//  SendBirdiOSSample
//
//  Created by SendBird Developers on 12/30/15.
//  Copyright © 2015 SENDBIRD.COM. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <QuartzCore/QuartzCore.h>

@interface ViewController : UIViewController

@property UIImageView *inteageLogoImageView;
@property UILabel *inteageLabel;

@property UIImageView *backgroundImageView;
@property UIButton *inteageChatStartButton;
@property UIButton *inteageChannelListButton;
@property UIButton *inteageLobbyMemberListButton;
@property UIButton *inteageMessagingChannelListButton;
@property UILabel *inteageUserNicknameLabel;
@property UITextField *inteageUserNicknameTextField;

- (UIImage *) imageFromColor:(UIColor *)color;

@end


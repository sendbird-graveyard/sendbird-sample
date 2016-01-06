//
//  ViewController.m
//  SendBirdiOSSample
//
//  Created by SendBird Developers on 12/30/15.
//  Copyright © 2015 SENDBIRD.COM. All rights reserved.
//

#import "ViewController.h"
#import "ChattingTableViewController.h"
#import "MessagingTableViewController.h"

@interface ViewController ()<UITextFieldDelegate>

@end

@implementation ViewController {
    NSString *messagingUserName;
    NSString *messagingUserId;
    NSString *messagingTargetUserId;
    BOOL startMessagingFromOpenChat;
}

- (void)viewDidLoad {
    [super viewDidLoad];
    
    [self initViews];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (UIImage *) imageFromColor:(UIColor *)color
{
    CGRect rect = CGRectMake(0, 0, 1, 1);
    UIGraphicsBeginImageContext(rect.size);
    CGContextRef context = UIGraphicsGetCurrentContext();
    CGContextSetFillColorWithColor(context, [color CGColor]);
    CGContextFillRect(context, rect);
    UIImage *img = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return img;
}

- (void) initViews
{
    [[self navigationController] setNavigationBarHidden:YES animated:NO];
    
    self.backgroundImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"_sendbird_img_bg_default.jpg"]];
    [self.backgroundImageView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.backgroundImageView setContentMode:UIViewContentModeScaleAspectFill];
    [self.backgroundImageView setClipsToBounds:YES];
    [self.view addSubview:self.backgroundImageView];
    
    // SendBird Logo
    self.inteageLogoImageView = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"_sendbird_icon_sendbird"]];
    [self.inteageLogoImageView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.view addSubview:self.inteageLogoImageView];
    
    // SendBird Label
    NSLog(@"Version: %@", [SendBird VERSION]);
    self.inteageLabel = [[UILabel alloc] init];
    [self.inteageLabel setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.inteageLabel setText:[NSString stringWithFormat:@"SendBird v%@", [SendBird VERSION]]];
    [self.inteageLabel setTextColor:[UIColor whiteColor]];
    [self.inteageLabel setFont:[UIFont boldSystemFontOfSize:28.0]];
    [self.inteageLabel setFont:[UIFont fontWithName:@"AmericanTypewriter-Bold" size:28]];
    [self.view addSubview:self.inteageLabel];
    
    // SendBird User Nickname Label
    self.inteageUserNicknameLabel = [[UILabel alloc] init];
    [self.inteageUserNicknameLabel setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.inteageUserNicknameLabel setText:@"Enter your nickname."];
    [self.inteageUserNicknameLabel setTextColor:[UIColor whiteColor]];
    [self.inteageUserNicknameLabel setFont:[UIFont boldSystemFontOfSize:16]];
    [self.view addSubview:self.inteageUserNicknameLabel];
    
    // SendBird User Nickname
    self.inteageUserNicknameTextField = [[UITextField alloc] init];
    [self.inteageUserNicknameTextField setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.inteageUserNicknameTextField setBackground:[self imageFromColor:UIColorFromRGB(0xE8EAF6)]];
    [self.inteageUserNicknameTextField setClipsToBounds:YES];
    [[self.inteageUserNicknameTextField layer] setCornerRadius:4];
    UIView *leftPaddingView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 12, 0)];
    UIView *rightPaddingView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, 12, 0)];
    self.inteageUserNicknameTextField.leftView = leftPaddingView;
    self.inteageUserNicknameTextField.leftViewMode = UITextFieldViewModeAlways;
    self.inteageUserNicknameTextField.rightView = rightPaddingView;
    self.inteageUserNicknameTextField.rightViewMode = UITextFieldViewModeAlways;
    [self.inteageUserNicknameTextField setPlaceholder:@"Nickname."];
    [self.inteageUserNicknameTextField setFont:[UIFont systemFontOfSize:16]];
    [self.inteageUserNicknameTextField setReturnKeyType:UIReturnKeyDone];
    [self.inteageUserNicknameTextField setDelegate:self];
    
    // Set Default User Nickname
    NSString *USER_ID = [SendBirdUtils deviceUniqueID];
    NSString *USER_NAME = [NSString stringWithFormat:@"User-%@", [USER_ID substringToIndex:5]];
    [self.inteageUserNicknameTextField setText:USER_NAME];
    
    [self.view addSubview:self.inteageUserNicknameTextField];
    
    // SendBird Open Lobby Channel for Sample
    self.inteageChatStartButton = [[UIButton alloc] init];
    [self.inteageChatStartButton setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.inteageChatStartButton setBackgroundImage:[self imageFromColor:UIColorFromRGB(0xAB47BC)] forState:UIControlStateNormal];
    [self.inteageChatStartButton setClipsToBounds:YES];
    [[self.inteageChatStartButton layer] setCornerRadius:4];
    [self.inteageChatStartButton addTarget:self action:@selector(clickSendBirdChatStartButton:) forControlEvents:UIControlEventTouchUpInside];
    [self.inteageChatStartButton setTitle:@"Join Lobby Channel" forState:UIControlStateNormal];
    [self.inteageChatStartButton setTitleColor:[UIColor whiteColor] forState:UIControlStateNormal];
    [self.inteageChatStartButton.titleLabel setFont:[UIFont boldSystemFontOfSize:16]];
    [self.view addSubview:self.inteageChatStartButton];
    
    // SendBird Open Channel List for Sample
    self.inteageChannelListButton = [[UIButton alloc] init];
    [self.inteageChannelListButton setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.inteageChannelListButton setBackgroundImage:[self imageFromColor:UIColorFromRGB(0xAB47BC)] forState:UIControlStateNormal];
    [self.inteageChannelListButton setClipsToBounds:YES];
    [[self.inteageChannelListButton layer] setCornerRadius:4];
    [self.inteageChannelListButton addTarget:self action:@selector(clickSendBirdChannelListButton:) forControlEvents:UIControlEventTouchUpInside];
    [self.inteageChannelListButton setTitle:@"Channel List" forState:UIControlStateNormal];
    [self.inteageChannelListButton setTitleColor:[UIColor whiteColor] forState:UIControlStateNormal];
    [self.inteageChannelListButton.titleLabel setFont:[UIFont boldSystemFontOfSize:16]];
    [self.view addSubview:self.inteageChannelListButton];
    
    // SendBird Open Member list of Lobby Channel
    self.inteageLobbyMemberListButton = [[UIButton alloc] init];
    [self.inteageLobbyMemberListButton setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.inteageLobbyMemberListButton setBackgroundImage:[self imageFromColor:UIColorFromRGB(0xAB47BC)] forState:UIControlStateNormal];
    [self.inteageLobbyMemberListButton setClipsToBounds:YES];
    [[self.inteageLobbyMemberListButton layer] setCornerRadius:4];
    [self.inteageLobbyMemberListButton addTarget:self action:@selector(clickSendBirdLobbyMemberListButton:) forControlEvents:UIControlEventTouchUpInside];
    [self.inteageLobbyMemberListButton setTitle:@"Start a Messaging" forState:UIControlStateNormal];
    [self.inteageLobbyMemberListButton setTitleColor:[UIColor whiteColor] forState:UIControlStateNormal];
    [self.inteageLobbyMemberListButton.titleLabel setFont:[UIFont boldSystemFontOfSize:16]];
    [self.view addSubview:self.inteageLobbyMemberListButton];
    
    // SendBird Open Messaging Channel List
    self.inteageMessagingChannelListButton = [[UIButton alloc] init];
    [self.inteageMessagingChannelListButton setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.inteageMessagingChannelListButton setBackgroundImage:[self imageFromColor:UIColorFromRGB(0xAB47BC)] forState:UIControlStateNormal];
    [self.inteageMessagingChannelListButton setClipsToBounds:YES];
    [[self.inteageMessagingChannelListButton layer] setCornerRadius:4];
    [self.inteageMessagingChannelListButton addTarget:self action:@selector(clickSendBirdMessagingChannelListButton:) forControlEvents:UIControlEventTouchUpInside];
    [self.inteageMessagingChannelListButton setTitle:@"Messaging Channel List" forState:UIControlStateNormal];
    [self.inteageMessagingChannelListButton setTitleColor:[UIColor whiteColor] forState:UIControlStateNormal];
    [self.inteageMessagingChannelListButton.titleLabel setFont:[UIFont boldSystemFontOfSize:16]];
    [self.view addSubview:self.inteageMessagingChannelListButton];
    
    // Background Image
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.backgroundImageView
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeTop
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.backgroundImageView
                                                          attribute:NSLayoutAttributeBottom
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeBottom
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.backgroundImageView
                                                          attribute:NSLayoutAttributeLeading
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeLeading
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.backgroundImageView
                                                          attribute:NSLayoutAttributeTrailing
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeTrailing
                                                         multiplier:1 constant:0]];
    
    // SendBird Logo
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLogoImageView
                                                          attribute:NSLayoutAttributeCenterX
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeCenterX
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLogoImageView
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeTop
                                                         multiplier:1 constant:48]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLogoImageView
                                                          attribute:NSLayoutAttributeWidth
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:80]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLogoImageView
                                                          attribute:NSLayoutAttributeHeight
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:80]];
    
    // SendBird Label
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLabel
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.inteageLogoImageView
                                                          attribute:NSLayoutAttributeBottom
                                                         multiplier:1 constant:8]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLabel
                                                          attribute:NSLayoutAttributeCenterX
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeCenterX
                                                         multiplier:1 constant:0]];
    
    // SendBird User Nickname Label
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageUserNicknameLabel
                                                          attribute:NSLayoutAttributeCenterX
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeCenterX
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageUserNicknameLabel
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.inteageLabel
                                                          attribute:NSLayoutAttributeBottom
                                                         multiplier:1 constant:20]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageUserNicknameLabel
                                                          attribute:NSLayoutAttributeWidth
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:220]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageUserNicknameLabel
                                                          attribute:NSLayoutAttributeHeight
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:36]];
    
    
    // SendBird User Nickname TextField
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageUserNicknameTextField
                                                          attribute:NSLayoutAttributeCenterX
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeCenterX
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageUserNicknameTextField
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.inteageUserNicknameLabel
                                                          attribute:NSLayoutAttributeBottom
                                                         multiplier:1 constant:4]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageUserNicknameTextField
                                                          attribute:NSLayoutAttributeWidth
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:220]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageUserNicknameTextField
                                                          attribute:NSLayoutAttributeHeight
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:36]];
    
    // SendBird Open Lobby Button.
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageChatStartButton
                                                          attribute:NSLayoutAttributeCenterX
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeCenterX
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageChatStartButton
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.inteageUserNicknameTextField
                                                          attribute:NSLayoutAttributeBottom
                                                         multiplier:1 constant:40]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageChatStartButton
                                                          attribute:NSLayoutAttributeWidth
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:220]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageChatStartButton
                                                          attribute:NSLayoutAttributeHeight
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:36]];
    
    // SendBird Open Lobby Button.
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageChannelListButton
                                                          attribute:NSLayoutAttributeCenterX
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeCenterX
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageChannelListButton
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.inteageChatStartButton
                                                          attribute:NSLayoutAttributeBottom
                                                         multiplier:1 constant:12]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageChannelListButton
                                                          attribute:NSLayoutAttributeWidth
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:220]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageChannelListButton
                                                          attribute:NSLayoutAttributeHeight
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:36]];
    // SendBird Member List at Lobby.
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLobbyMemberListButton
                                                          attribute:NSLayoutAttributeCenterX
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeCenterX
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLobbyMemberListButton
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.inteageChannelListButton
                                                          attribute:NSLayoutAttributeBottom
                                                         multiplier:1 constant:12]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLobbyMemberListButton
                                                          attribute:NSLayoutAttributeWidth
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:220]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageLobbyMemberListButton
                                                          attribute:NSLayoutAttributeHeight
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:36]];
    
    // SendBird Messaging Channel List.
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageMessagingChannelListButton
                                                          attribute:NSLayoutAttributeCenterX
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.view
                                                          attribute:NSLayoutAttributeCenterX
                                                         multiplier:1 constant:0]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageMessagingChannelListButton
                                                          attribute:NSLayoutAttributeTop
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:self.inteageLobbyMemberListButton
                                                          attribute:NSLayoutAttributeBottom
                                                         multiplier:1 constant:12]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageMessagingChannelListButton
                                                          attribute:NSLayoutAttributeWidth
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:220]];
    [self.view addConstraint:[NSLayoutConstraint constraintWithItem:self.inteageMessagingChannelListButton
                                                          attribute:NSLayoutAttributeHeight
                                                          relatedBy:NSLayoutRelationEqual
                                                             toItem:nil
                                                          attribute:NSLayoutAttributeNotAnAttribute
                                                         multiplier:1 constant:36]];
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    //    startMessagingFromOpenChat = NO;
    [[UIApplication sharedApplication] setStatusBarStyle:UIStatusBarStyleLightContent];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(startMessagingWithUser:) name:@"open_messaging" object:nil];
}

- (void)viewDidAppear:(BOOL)animated
{
    if (startMessagingFromOpenChat == YES) {
        MessagingTableViewController *viewController = [[MessagingTableViewController alloc] init];
        
        [viewController setViewMode:kMessagingViewMode];
        [viewController initChannelTitle];
        [viewController setChannelUrl:@""];
        [viewController setUserName:messagingUserName];
        [viewController setUserId:messagingUserId];
        [viewController setTargetUserId:messagingTargetUserId];
        
#if 0
        [self.navigationController setModalPresentationStyle:UIModalPresentationCurrentContext];
        UINavigationController *navController = [[UINavigationController alloc] initWithRootViewController:viewController];
        [navController setModalPresentationStyle:UIModalPresentationCurrentContext];
        [self.navigationController presentViewController:navController animated:YES completion:nil];
#else
        UINavigationController *navigationController = [[UINavigationController alloc] initWithRootViewController:viewController];
        [self presentViewController:navigationController animated:YES completion: nil];
#endif
    }
    
    startMessagingFromOpenChat = NO;
}

- (void)startMessagingWithUser:(NSNotification *)obj {
    messagingTargetUserId = (NSString *)[obj object];
    startMessagingFromOpenChat = YES;
}

- (void)clickSendBirdChatStartButton:(id)sender
{
    if ([[self.inteageUserNicknameTextField text] length] > 0) {
        [self startSendBirdWithUserName:[self.inteageUserNicknameTextField text] andChatMode:kChatModeChatting andViewMode:(int)kChattingViewMode];
    }
}

- (void)clickSendBirdChannelListButton:(id)sender
{
    if ([[self.inteageUserNicknameTextField text] length] > 0) {
        [self startSendBirdWithUserName:[self.inteageUserNicknameTextField text] andChatMode:kChatModeChatting andViewMode:(int)kChannelListViewMode];
    }
}

- (void)clickSendBirdLobbyMemberListButton:(id)sender
{
    if ([[self.inteageUserNicknameTextField text] length] > 0) {
        [self startSendBirdWithUserName:[self.inteageUserNicknameTextField text] andChatMode:kChatModeMessaging andViewMode:(int)kMessagingMemberViewMode];
    }
}

- (void)clickSendBirdMessagingChannelListButton:(id)sender
{
    if ([[self.inteageUserNicknameTextField text] length] > 0) {
        [self startSendBirdWithUserName:[self.inteageUserNicknameTextField text] andChatMode:kChatModeMessaging andViewMode:(int)kMessagingChannelListViewMode];
    }
}

- (void) startSendBirdWithUserName:(NSString *)userName andChatMode:(int)chatMode andViewMode:(int)viewMode
{
    NSString *APP_ID = @"A7A2672C-AD11-11E4-8DAA-0A18B21C2D82";
    NSString *USER_ID = [SendBirdUtils deviceUniqueID];
    NSString *USER_NAME = userName;
    NSString *CHANNEL_URL = @"jia_test.Lobby";
    
    messagingUserName = USER_NAME;
    messagingUserId = USER_ID;
    
    if (chatMode == kChatModeChatting) {
        ChattingTableViewController *viewController = [[ChattingTableViewController alloc] init];
        
        [SendBird initAppId:APP_ID withDeviceId:[SendBird deviceUniqueID]];
        
        [viewController setViewMode:viewMode];
        [viewController initChannelTitle];
        [viewController setChannelUrl:CHANNEL_URL];
        [viewController setUserName:USER_NAME];
        [viewController setUserId:USER_ID];
        
#if 0
        [self.navigationController setModalPresentationStyle:UIModalPresentationCurrentContext];
        UINavigationController *navController = [[UINavigationController alloc] initWithRootViewController:viewController];
        [navController setModalPresentationStyle:UIModalPresentationCurrentContext];
        [self.navigationController presentViewController:navController animated:YES completion:nil];
#else
        [self.navigationController pushViewController:viewController animated:NO];
#endif
    }
    else if (chatMode == kChatModeMessaging) {
        MessagingTableViewController *viewController = [[MessagingTableViewController alloc] init];
        
        [SendBird initAppId:APP_ID withDeviceId:[SendBird deviceUniqueID]];
        
        [viewController setViewMode:viewMode];
        [viewController initChannelTitle];
        [viewController setChannelUrl:CHANNEL_URL];
        [viewController setUserName:USER_NAME];
        [viewController setUserId:USER_ID];
        
#if 0
        [self.navigationController setModalPresentationStyle:UIModalPresentationCurrentContext];
        UINavigationController *navController = [[UINavigationController alloc] initWithRootViewController:viewController];
        [navController setModalPresentationStyle:UIModalPresentationCurrentContext];
        [self.navigationController presentViewController:navController animated:YES completion:nil];
#else
        [self.navigationController pushViewController:viewController animated:NO];
#endif
    }
}

#pragma mark - UITextFieldDelegate
- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    [textField resignFirstResponder];
    return YES;
}

@end

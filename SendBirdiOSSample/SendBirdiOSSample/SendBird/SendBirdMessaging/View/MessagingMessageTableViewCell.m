//
//  MessagingMessageTableViewCell.m
//  SendBirdiOSSample
//
//  Created by SendBird Developers on 2015. 7. 29..
//  Copyright (c) 2015 SENDBIRD.COM. All rights reserved.
//

#import "MessagingMessageTableViewCell.h"

#define kMessageCellTopMargin 14
#define kMessageCellBottomMargin 0
#define kMessageCellLeftMargin 12
#define kMessageFontSize 14.0
#define kMessageBalloonTopPadding 12
#define kMessageBalloonBottomPadding 12
#define kMessageBalloonLeftPadding 60
#define kMessageBalloonRightPadding 12
#define kMessageMaxWidth 168
#define kMessageProfileHeight 36
#define kMessageProfileWidth 36
#define kMessageDateTimeLeftMarign 4
#define kMessageDateTimeFontSize 10.0
#define kMessageNicknameFontSize 12.0

@implementation MessagingMessageTableViewCell {
    CGFloat topMargin;
}

- (id)initWithStyle:(UITableViewCellStyle)style reuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:style reuseIdentifier:reuseIdentifier];
    
    if (self != nil) {
        topMargin = kMessageCellTopMargin;
        [self initViews];
    }
    
    return self;
}

- (void) initViews
{
    [self setBackgroundColor:[UIColor clearColor]];
    
    self.profileImageView = [[UIImageView alloc] init];
    [self.profileImageView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.profileImageView.layer setCornerRadius:(kMessageProfileHeight / 2)];
    [self.profileImageView setClipsToBounds:YES];
    [self.profileImageView setContentMode:UIViewContentModeScaleAspectFill];
    [self addSubview:self.profileImageView];
    
    self.messageBackgroundImageView = [[UIImageView alloc] init];
    [self.messageBackgroundImageView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.messageBackgroundImageView setImage:[UIImage imageNamed:@"_bg_chat_bubble_gray"]];
    [self addSubview:self.messageBackgroundImageView];
    
    self.nicknameLabel = [[UILabel alloc] init];
    [self.nicknameLabel setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.nicknameLabel setFont:[UIFont systemFontOfSize:kMessageNicknameFontSize]];
    [self.nicknameLabel setNumberOfLines:1];
    [self.nicknameLabel setTextColor:UIColorFromRGB(0xa792e5)];
    [self.nicknameLabel setLineBreakMode:NSLineBreakByCharWrapping];
    [self addSubview:self.nicknameLabel];
    
    self.messageLabel = [[UILabel alloc] init];
    [self.messageLabel setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.messageLabel setFont:[UIFont systemFontOfSize:kMessageFontSize]];
    [self.messageLabel setNumberOfLines:0];
    [self.messageLabel setTextColor:UIColorFromRGB(0x3d3d3d)];
    [self.messageLabel setLineBreakMode:NSLineBreakByCharWrapping];
    [self addSubview:self.messageLabel];
    
    self.dateTimeLabel = [[UILabel alloc] init];
    [self.dateTimeLabel setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.dateTimeLabel setNumberOfLines:1];
    [self.dateTimeLabel setTextColor:UIColorFromRGB(0xacaab2)];
    [self.dateTimeLabel setFont:[UIFont systemFontOfSize:kMessageDateTimeFontSize]];
    [self.dateTimeLabel setText:@"11:24 PM"];
    [self addSubview:self.dateTimeLabel];
    
    // Profile Image View
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.profileImageView
                                                     attribute:NSLayoutAttributeBottom
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeBottom
                                                    multiplier:1 constant:-kMessageCellBottomMargin]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.profileImageView
                                                     attribute:NSLayoutAttributeLeading
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeLeading
                                                    multiplier:1 constant:kMessageCellLeftMargin]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.profileImageView
                                                     attribute:NSLayoutAttributeWidth
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:nil
                                                     attribute:NSLayoutAttributeNotAnAttribute
                                                    multiplier:1 constant:kMessageProfileWidth]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.profileImageView
                                                     attribute:NSLayoutAttributeHeight
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:nil
                                                     attribute:NSLayoutAttributeNotAnAttribute
                                                    multiplier:1 constant:kMessageProfileHeight]];
    
    // Nickname Label
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.nicknameLabel
                                                     attribute:NSLayoutAttributeBottom
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self.messageLabel
                                                     attribute:NSLayoutAttributeTop
                                                    multiplier:1 constant:0]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.nicknameLabel
                                                     attribute:NSLayoutAttributeLeading
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeLeading
                                                    multiplier:1 constant:kMessageBalloonLeftPadding]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.nicknameLabel
                                                     attribute:NSLayoutAttributeWidth
                                                     relatedBy:NSLayoutRelationLessThanOrEqual
                                                        toItem:nil
                                                     attribute:NSLayoutAttributeNotAnAttribute
                                                    multiplier:1 constant:kMessageMaxWidth]];
    
    // Message Label
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.messageLabel
                                                     attribute:NSLayoutAttributeBottom
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeBottom
                                                    multiplier:1 constant:-kMessageBalloonBottomPadding - kMessageCellBottomMargin]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.messageLabel
                                                     attribute:NSLayoutAttributeLeading
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeLeading
                                                    multiplier:1 constant:kMessageBalloonLeftPadding]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.messageLabel
                                                     attribute:NSLayoutAttributeWidth
                                                     relatedBy:NSLayoutRelationLessThanOrEqual
                                                        toItem:nil
                                                     attribute:NSLayoutAttributeNotAnAttribute
                                                    multiplier:1 constant:kMessageMaxWidth]];
    
    // Message Background Image View
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.messageBackgroundImageView
                                                     attribute:NSLayoutAttributeBottom
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeBottom
                                                    multiplier:1 constant:-kMessageCellBottomMargin]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.messageBackgroundImageView
                                                     attribute:NSLayoutAttributeLeading
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeLeading
                                                    multiplier:1 constant:kMessageBalloonLeftPadding - 16]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.messageBackgroundImageView
                                                     attribute:NSLayoutAttributeTrailing
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self.messageLabel
                                                     attribute:NSLayoutAttributeTrailing
                                                    multiplier:1 constant:kMessageBalloonRightPadding]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.messageBackgroundImageView
                                                     attribute:NSLayoutAttributeTrailing
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self.nicknameLabel
                                                     attribute:NSLayoutAttributeTrailing
                                                    multiplier:1 constant:kMessageBalloonRightPadding]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.messageBackgroundImageView
                                                     attribute:NSLayoutAttributeTop
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self.nicknameLabel
                                                     attribute:NSLayoutAttributeTop
                                                    multiplier:1 constant:-kMessageBalloonTopPadding]];
    
    // DateTime Label
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.dateTimeLabel
                                                     attribute:NSLayoutAttributeBottom
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self.messageBackgroundImageView
                                                     attribute:NSLayoutAttributeBottom
                                                    multiplier:1 constant:0]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.dateTimeLabel
                                                     attribute:NSLayoutAttributeLeading
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self.messageBackgroundImageView
                                                     attribute:NSLayoutAttributeTrailing
                                                    multiplier:1 constant:kMessageDateTimeLeftMarign]];
}

- (void) setContinuousMessage:(BOOL)continuousFlag
{
    if (continuousFlag) {
        topMargin = 4.0;
    }
    else {
        topMargin = kMessageCellTopMargin;
    }
}

- (void) setModel:(SendBirdMessage *)message
{
    self.message = message;
    [self.messageLabel setAttributedText:[self buildMessage]];
    SendBirdSender *sender = [self.message sender];
    long long ts = [self.message getMessageTimestamp] / 1000;
    [self.dateTimeLabel setText:[SendBirdUtils messageDateTime:ts]];
    [self.nicknameLabel setText:[sender name]];
    
#ifdef __WITH_AFNETWORKING__
    [self.profileImageView setImageWithURL:[NSURL URLWithString:[sender imageUrl]]];
#else
#warning THIS IS SAMPLE CODE. Do not use ImageCache in your product. Use your own image loader or 3rd party image loader.
    UIImage *image = [[ImageCache sharedInstance] getImage:[sender imageUrl]];
    if (image) {
        @try {
            [self.profileImageView setImage:image];
        }
        @catch (NSException *exception) {
            NSLog(@"FileLink Exception");
        }
        @finally {
        }
    }
    else {
        [SendBirdUtils imageDownload:[NSURL URLWithString:[sender imageUrl]] endBlock:^(NSData *response, NSError *error) {
            UIImage *image = [[UIImage alloc] initWithData:response scale:1];
            UIImage *newImage = [SendBirdUtils imageWithImage:image scaledToSize:kMessageProfileWidth];
            
            [[ImageCache sharedInstance] setImage:newImage withKey:[sender imageUrl]];
            @try {
                dispatch_queue_t queue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0ul);
                dispatch_async(queue, ^(void) {
                    dispatch_async(dispatch_get_main_queue(), ^{
                        [self.profileImageView setImage:newImage];
                    });
                });
            }
            @catch (NSException *exception) {
                NSLog(@"FileLink Exception");
            }
            @finally {
            }
        }];
    }
    [self setNeedsLayout];
#endif
}

- (NSAttributedString *)buildMessage
{
    NSMutableDictionary *messageAttribute = [NSMutableDictionary dictionaryWithObjectsAndKeys:[UIFont systemFontOfSize:kMessageFontSize], NSFontAttributeName, UIColorFromRGB(0x3d3d3d), NSForegroundColorAttributeName,nil];
    NSMutableDictionary *urlAttribute = [NSMutableDictionary dictionaryWithObjectsAndKeys:[UIFont systemFontOfSize:kMessageFontSize], NSFontAttributeName, UIColorFromRGB(0x2981e1), NSForegroundColorAttributeName,nil];
    NSString *blockMark = @"";
//    if ([self.message isBlocked]) {
//        blockMark = @"Blocked";
//    }
//    else {
//        blockMark = @"Unblocked";
//    }
    NSString *message = [[NSString stringWithFormat:@"%@%@", [self.message message], blockMark] stringByReplacingOccurrencesOfString:@" " withString:@"\u00A0"];
    NSString *url = [SendBirdUtils getUrlFromString:[self.message message]];
    NSRange urlRange;
    if ([url length] > 0) {
        urlRange = [message rangeOfString:url];
    }
    message = [message stringByReplacingOccurrencesOfString:@" " withString:@"\u00A0"];
    message = [message stringByReplacingOccurrencesOfString:@"-" withString:@"\u2011"];
    
    NSMutableAttributedString *attributedMessage = [[NSMutableAttributedString alloc] initWithString:message];
    NSRange messageRange = NSMakeRange(0, [[self.message message] length]);
    
    [attributedMessage beginEditing];
    [attributedMessage setAttributes:messageAttribute range:messageRange];
    if ([url length] > 0) {
        [attributedMessage setAttributes:urlAttribute range:urlRange];
    }
    [attributedMessage endEditing];
    
    return attributedMessage;
}

- (CGFloat)getHeightOfViewCell:(CGFloat)totalWidth
{
    NSString *nickname = [[self.message sender] name];
    CGRect messageRect;
    CGRect nicknameRect;
    NSAttributedString *attributedMessage = [self buildMessage];
    NSMutableDictionary *nicknameAttribute = [NSMutableDictionary dictionaryWithObjectsAndKeys:[UIFont systemFontOfSize:12], NSFontAttributeName, nil];
    NSAttributedString *attributedNickname = [[NSAttributedString alloc] initWithString:nickname attributes:nicknameAttribute];
    
    messageRect = [attributedMessage boundingRectWithSize:CGSizeMake(kMessageMaxWidth, CGFLOAT_MAX) options:(NSStringDrawingUsesLineFragmentOrigin) context:nil];
    nicknameRect = [attributedNickname boundingRectWithSize:CGSizeMake(kMessageMaxWidth, CGFLOAT_MAX) options:(NSStringDrawingUsesLineFragmentOrigin) context:nil];
    
    CGFloat height = nicknameRect.size.height + messageRect.size.height + kMessageCellTopMargin + kMessageCellBottomMargin + kMessageBalloonBottomPadding + kMessageBalloonTopPadding;
    
    return height;
}

@end

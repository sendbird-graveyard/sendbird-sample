//
//  MemberTableViewCell.h
//  SendBirdiOSSample
//
//  Created by SendBird Developers on 2015. 7. 29..
//  Copyright (c) 2015 SENDBIRD.COM. All rights reserved.
//

#import "MemberTableViewCell.h"

@implementation MemberTableViewCell

- (id)initWithStyle:(UITableViewCellStyle)style reuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:style reuseIdentifier:reuseIdentifier];
    
    if (self != nil) {
        [self initViews];
    }
    
    return self;
}

- (void) initViews
{
    [self setSelectionStyle:UITableViewCellSelectionStyleNone];
    
    self.profileImageView = [[UIImageView alloc] init];
    [self.profileImageView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.profileImageView setClipsToBounds:YES];
    [[self.profileImageView layer] setCornerRadius:20];
    [self.contentView addSubview:self.profileImageView];
    
    self.nicknameLabel = [[UILabel alloc] init];
    [self.nicknameLabel setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.nicknameLabel setFont:[UIFont systemFontOfSize:14]];
    [self.nicknameLabel setTextColor:UIColorFromRGB(0x3d3d3d)];
    [self.contentView addSubview:self.nicknameLabel];
    
    self.checkImageView = [[UIImageView alloc] init];
    [self.checkImageView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.checkImageView setImage:[UIImage imageNamed:@"_check_member_off"]];
    [self addSubview:self.checkImageView];
    
    self.seperateLineView = [[UIView alloc] init];
    [self.seperateLineView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [self.seperateLineView setBackgroundColor:UIColorFromRGB(0xc8c8c8)];
    [self.contentView addSubview:self.seperateLineView];
    
    [self applyConstraints];
}

- (void) applyConstraints
{
    // Profile ImageView
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.profileImageView
                                                                 attribute:NSLayoutAttributeCenterY
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:self.contentView
                                                                 attribute:NSLayoutAttributeCenterY
                                                                multiplier:1 constant:0]];
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.profileImageView
                                                                 attribute:NSLayoutAttributeLeading
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:self.contentView
                                                                 attribute:NSLayoutAttributeLeading
                                                                multiplier:1 constant:12]];
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.profileImageView
                                                                 attribute:NSLayoutAttributeWidth
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:nil
                                                                 attribute:NSLayoutAttributeNotAnAttribute
                                                                multiplier:1 constant:40]];
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.profileImageView
                                                                 attribute:NSLayoutAttributeHeight
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:nil
                                                                 attribute:NSLayoutAttributeNotAnAttribute
                                                                multiplier:1 constant:40]];
    
    // Nickname Label
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.nicknameLabel
                                                                 attribute:NSLayoutAttributeCenterY
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:self.contentView
                                                                 attribute:NSLayoutAttributeCenterY
                                                                multiplier:1 constant:0]];
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.nicknameLabel
                                                                 attribute:NSLayoutAttributeLeading
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:self.profileImageView
                                                                 attribute:NSLayoutAttributeTrailing
                                                                multiplier:1 constant:14]];
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.nicknameLabel
                                                                 attribute:NSLayoutAttributeTrailing
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:self.contentView
                                                                 attribute:NSLayoutAttributeTrailing
                                                                multiplier:1 constant:-12]];
    
    // Check Image View
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.checkImageView
                                                     attribute:NSLayoutAttributeTrailing
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeTrailing
                                                    multiplier:1 constant:-12]];
    [self addConstraint:[NSLayoutConstraint constraintWithItem:self.checkImageView
                                                     attribute:NSLayoutAttributeCenterY
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:self
                                                     attribute:NSLayoutAttributeCenterY
                                                    multiplier:1 constant:0]];
    
    // Seperator Line View
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.seperateLineView
                                                                 attribute:NSLayoutAttributeBottom
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:self.contentView
                                                                 attribute:NSLayoutAttributeBottom
                                                                multiplier:1 constant:0]];
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.seperateLineView
                                                                 attribute:NSLayoutAttributeLeading
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:self.contentView
                                                                 attribute:NSLayoutAttributeLeading
                                                                multiplier:1 constant:66]];
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.seperateLineView
                                                                 attribute:NSLayoutAttributeTrailing
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:self.contentView
                                                                 attribute:NSLayoutAttributeTrailing
                                                                multiplier:1 constant:0]];
    [self.contentView addConstraint:[NSLayoutConstraint constraintWithItem:self.seperateLineView
                                                                 attribute:NSLayoutAttributeHeight
                                                                 relatedBy:NSLayoutRelationEqual
                                                                    toItem:nil
                                                                 attribute:NSLayoutAttributeNotAnAttribute
                                                                multiplier:1 constant:0.5]];
}

- (void) setSelected:(BOOL)selected
{
    [super setSelected:selected];
    
    if (selected) {
        [self.checkImageView setImage:[UIImage imageNamed:@"_check_member_on"]];
    }
    else {
        [self.checkImageView setImage:[UIImage imageNamed:@"_check_member_off"]];
    }
}

- (void) setModel:(SendBirdAppUser *)model withCheckMark:(BOOL)check
{
    if (check) {
        [self.checkImageView setHidden:NO];
    }
    else {
        [self.checkImageView setHidden:YES];
    }
    
    [self.nicknameLabel setText:[model nickname]];
#ifdef __WITH_AFNETWORKING__
    [self.profileImageView setImageWithURL:[NSURL URLWithString:[model imageUrl]]];
#else
#warning THIS IS SAMPLE CODE. Do not use ImageCache in your product. Use your own image loader or 3rd party image loader.
    UIImage *image = [[ImageCache sharedInstance] getImage:[model picture]];
    if (image) {
        @try {
            [self.profileImageView setImage:image];
        }
        @catch (NSException *exception) {
            NSLog(@"Channel Exception");
        }
        @finally {
        }
    }
    else {
        [SendBirdUtils imageDownload:[NSURL URLWithString:[model picture]] endBlock:^(NSData *response, NSError *error) {
            UIImage *image = [[UIImage alloc] initWithData:response scale:1];
            UIImage *newImage = [SendBirdUtils imageWithImage:image scaledToSize:40];
            
            [[ImageCache sharedInstance] setImage:newImage withKey:[model picture]];
            @try {
                dispatch_queue_t queue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0ul);
                dispatch_async(queue, ^(void) {
                    dispatch_async(dispatch_get_main_queue(), ^{
                        [self.profileImageView setImage:newImage];
                    });
                });
            }
            @catch (NSException *exception) {
                NSLog(@"Channel Exception");
            }
            @finally {
            }
        }];
    }
    [self setNeedsLayout];
#endif
}

@end

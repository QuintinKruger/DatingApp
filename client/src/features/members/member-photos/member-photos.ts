import { Component, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/member-service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Member, Photo } from '../../../types/member';
import { AsyncPipe } from '@angular/common';
import { ImageUpload } from '../../../shared/image-upload/image-upload';
import { provideClientHydration } from '@angular/platform-browser';
import { AccountService } from '../../../core/services/account-service';
import { User } from '../../../types/user';
import { StarButton } from '../../../shared/star-button/star-button';
import { DeleteButton } from '../../../shared/delete-button/delete-button';

@Component({
  selector: 'app-member-photos',
  imports: [AsyncPipe, ImageUpload, StarButton, DeleteButton],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css',
})
export class MemberPhotos implements OnInit {
  protected memberService = inject(MemberService);
  protected accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  protected photos = signal<Photo[]>([]);
  protected loading = signal(false);

  ngOnInit(): void {
    // this component is a child of MemberDetailed where the path of the parent 
    // contains the id parameter, we can thus query the parent to get the route param
    const memberId = this.route.parent?.snapshot.paramMap.get('id')
    if (memberId) {
      this.memberService.getMemberPhotos(memberId).subscribe({
        next: (photos) => this.photos.set(photos)
      })
    }
  }

  // javascript getter so that we can access photoMocks as if it were a property 
  get photoMocks() {
    return Array.from({ length: 20 }, (_, i) => ({ url: '/user.png' }))
  }

  onUploadImage(file: File) {
    this.loading.set(true);
    this.memberService.uploadPhoto(file).subscribe({
      next: (photo) => {
        this.memberService.editMode.set(false);
        this.loading.set(false);
        this.photos.update(current => [...current, photo]) // load all curent image and the new one just uploaded
        if (!this.memberService.member()?.imageUrl) {
          this.setMainLocalPhoto(photo)
        }
      },
      error: (err) => {
        console.log("Error uploading image: " + err);
        this.loading.set(false)
      }
    })
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: () => {
        this.setMainLocalPhoto(photo)
      }
    })
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        this.photos.update(current => current.filter(x => x.id != photoId))
      }
    })
  }

  private setMainLocalPhoto(photo: Photo) {
    const currentuser = this.accountService.currentUser();
    if (currentuser) currentuser.imageUrl = photo.url;
    this.accountService.setCurrentUser(currentuser as User);
    this.memberService.member.update(current => {
      return {
        ...current,
        imageUrl: photo.url
      } as Member
    })
  }
}

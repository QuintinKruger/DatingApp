import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../../core/admin-service';
import { Photo } from '../../../types/member';
import { PaginatedResult } from '../../../types/pagination';

@Component({
  selector: 'app-photo-management',
  imports: [],
  templateUrl: './photo-management.html',
  styleUrl: './photo-management.css',
})
export class PhotoManagement implements OnInit {
  private adminService = inject(AdminService)
  protected paginatedUnapprovedPhotos = signal<PaginatedResult<Photo> | null>(null)
  protected pageSize = 5;
  protected pageNumber = 1;

  ngOnInit(): void {
    this.getPhotosForApproval()
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval(this.pageSize, this.pageNumber).subscribe({
      next: result => this.paginatedUnapprovedPhotos.set(result),
      error: err => console.log("Some error", err)
    })
  }

  approvePhoto(photo: Photo) {
    this.adminService.approvePhoto(photo).subscribe({
      next: () =>  this.paginatedUnapprovedPhotos.update(paginatedUnapprovedPhotos => {
        // dont make another call to get updated photos - simply remove our image we just 
        // approved from the list
        return {
          ...paginatedUnapprovedPhotos,
          items: paginatedUnapprovedPhotos?.items.filter(x => x.id !== photo.id)
        } as PaginatedResult<Photo>
      })
    })
   
  }

  rejectPhoto(photo: Photo) {
    this.adminService.rejectPhoto(photo).subscribe({
      next: () =>  this.paginatedUnapprovedPhotos.update(paginatedUnapprovedPhotos => {
        // dont make another call to get updated photos - simply remove our image we just 
        // rejected from the list
        return {
          ...paginatedUnapprovedPhotos,
          items: paginatedUnapprovedPhotos?.items.filter(x => x.id !== photo.id)
        } as PaginatedResult<Photo>
      })
    })
  }
}

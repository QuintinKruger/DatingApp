import { inject, Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../types/user';
import { Photo } from '../types/member';
import { PaginatedResult } from '../types/pagination';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);

  getUserWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles')
  }

  updateUserRoles(userId: string, roles: string[]) {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + userId + '?roles=' + roles, {})
  }

  getPhotosForApproval(pageSize: number, pageNumber: number) {
    return this.http.get<PaginatedResult<Photo>>(this.baseUrl + `admin/photos-to-moderate?pageSize=${pageSize}&pageNumber=${pageNumber}`)
  }

  approvePhoto(photo: Photo) {
    return this.http.post(this.baseUrl + `admin/approve-photo/${photo.id}`, {})
  }

  rejectPhoto(photo: Photo) {
    return this.http.post(this.baseUrl + `admin/reject-photo/${photo.id}`, {})
  }


}

import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, pipe } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  userParams!: UserParams;
  user!: User; 

  constructor(private http:HttpClient, private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(_user => {
      this.user = _user;
      this.userParams = new UserParams(_user);
    })
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) { 
      var response = this.memberCache.get(Object.values(userParams).join("-"));
      if(response) {
        return of(response);
      }
      
      let params = this.getPaginationHeader(userParams.pageNumber,userParams.pageSize);
      params = params.append('minAge',userParams.minAge.toString());
      params = params.append('maxAge',userParams.maxAge.toString());
      params = params.append('gender',userParams.gender);
      params = params.append('orderBy',userParams.orderBy);

      return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)
        .pipe(map(response => {
            this.memberCache.set(Object.values(userParams).join("-"),response);
            return response;
        }));
    }  

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result),[])
      .find((member: Member) => member.username === username);

    return this.http.get<Member>(this.baseUrl + "users/" + username);
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl + 'users',member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }
  
  setMainPhoto(photoId: Number){
    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId,{});
  }

  deletePhoto(photoId: Number){
    return this.http.delete(this.baseUrl + "users/delete-photo/" + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + "likes/" + username,{});
  }

  getLikes(predicate: string,pageNumber: Number, pageSize: Number) {
    let params = this.getPaginationHeader(pageNumber, pageSize);
    params = params.append("predicate",predicate);
    return this.getPaginatedResult<Partial<Member[]>>(this.baseUrl + "likes", params);
  }

  private getPaginatedResult<T>(url : any ,params: any) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url , { observe: 'response', params }).pipe(
      map(response => {
        if (response.body !== null)
          paginatedResult.result = response.body;
        var pg = response.headers.get('Pagination');
        if (pg != null) {
          paginatedResult.pagination = JSON.parse(pg);
        }
        return paginatedResult;
      })
    );
  } 

  private getPaginationHeader(pageNumber: Number, pageSize: Number) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    return params;
  }
}

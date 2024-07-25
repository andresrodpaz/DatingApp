import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs";
import { PaginatedResult } from "../_models/pagination";

/**
 * Retrieves a paginated result from the API.
 * @param url - The URL of the API endpoint to fetch data from.
 * @param params - The HTTP parameters to be sent with the request.
 * @param http - An instance of HttpClient for making HTTP requests.
 * @returns An observable of a PaginatedResult containing the result and pagination information.
 */
export function getPaginatedResult<T>(url: string, params: HttpParams, http: HttpClient) {
  const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
  return http.get<T>(url, { observe: 'response', params }).pipe(
    map((response) => {
      if (response.body) {
        paginatedResult.result = response.body;
      }
      const pagination = response.headers.get('Pagination');
      if (pagination) {
        paginatedResult.pagination = JSON.parse(pagination);
      }
      return paginatedResult;
    })
  );
}

/**
 * Constructs the HTTP parameters for pagination.
 * @param pageNumber - The page number to request.
 * @param pageSize - The number of items per page.
 * @returns An instance of HttpParams with pagination parameters.
 */
export function getPaginationHeaders(pageNumber: number, pageSize: number) {
  let params = new HttpParams();

  params = params.append('pageNumber', pageNumber);
  params = params.append('pageSize', pageSize);

  return params;
}

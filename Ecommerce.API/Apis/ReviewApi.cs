using Ecommerce.API.Services;
using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Apis
{
    public static class ReviewApi
    {
        public static IEndpointRouteBuilder MapReviewApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            // Get all reviews
            v1.MapGet("/reviews", (IReviewService reviewService) => reviewService.GetAllReviewsAsync());

            // Get review by id
            v1.MapGet("/reviews/{reviewId:guid}", async (IReviewService reviewService, Guid reviewId) =>
            {
                var review = await reviewService.GetReviewByIdAsync(reviewId);
                return review is not null ? Results.Ok(review) : Results.NotFound();
            });

            // Get reviews by product id
            v1.MapGet("/reviews/product/{productId:guid}", async (IReviewService reviewService, Guid productId) =>
            {
                var reviews = await reviewService.GetReviewsByProductIdAsync(productId);
                return Results.Ok(reviews);
            });

            // Get reviews by user id
            v1.MapGet("/reviews/user/{userId:guid}", async (IReviewService reviewService, Guid userId) =>
            {
                var reviews = await reviewService.GetReviewsByUserIdAsync(userId);
                return Results.Ok(reviews);
            });

            ////// Check if user has reviewed product
            //v1.MapGet("/reviews/user/{userId:guid}/product/{productId:guid}/has-reviewed", async (IReviewService reviewService, Guid userId, Guid productId) =>
            //{
            //    var hasReviewed = await reviewService.UserCanReviewProductAsync(userId, productId);
            //    return Results.Ok(hasReviewed);
            //});

            //Check if order is completed for product

            v1.MapGet("/reviews/user/{userId:guid}/product/{productId:guid}/order-completed", async (IReviewService reviewService, Guid userId, Guid productId) =>
            {
                var isOrderCompleted = await reviewService.IsOrderCompletedForProductAsync(userId, productId);
                return Results.Ok(isOrderCompleted);
            });

           // Create a new review
            v1.MapPost("/reviews", async (
                IReviewService reviewService, 
                [FromForm] CreateReviewDto review, 
                [FromServices] IHubContext<ReviewHub> hubContext) =>
            {
               
                var createdReview = await reviewService.CreateReviewAsync(review);
                if (createdReview is null)
                {
                    return Results.BadRequest("User has already reviewed this product or cannot review it.");
                }
                await hubContext.Clients.Group(review.ProductId.ToString()).SendAsync(
                        "ReceiveReview",
                        createdReview
                );
                var averageRating = await reviewService.GetAverageRatingByProductId(review.ProductId);
                var totalReviews = await reviewService.GetTotalReviewsByProductId(review.ProductId);
                await hubContext.Clients.Group(review.ProductId.ToString()).SendAsync(
                    "UpdateReviewStats",
                    new { averageRating, totalReviews }
                );
                return Results.Ok(createdReview);
            }).Accepts<CreateReviewDto>("multipart/form-data")
            .DisableAntiforgery();


            // Update a review
            v1.MapPut("/reviews/{reviewId:guid}", async (IReviewService reviewService, [FromForm] UpdateReviewDto review, Guid reviewId) =>
            {
                var existingReview = await reviewService.GetReviewByIdAsync(reviewId);
                if (existingReview is null)
                {
                    return Results.NotFound();
                }
                var updatedReview = await reviewService.UpdateReviewAsync(reviewId ,review);
                return updatedReview is not null ? Results.Ok(updatedReview) : Results.NotFound();
            });

            // Delete a review
            v1.MapDelete("/reviews/{reviewId:guid}", async (IReviewService reviewService, Guid reviewId) =>
            {
                var result = await reviewService.DeleteReviewAsync(reviewId);
                return result ? Results.NoContent() : Results.NotFound();
            });

            // Get average rating by product id
            v1.MapGet("/reviews/product/{productId:guid}/average-rating", async (IReviewService reviewService, Guid productId) =>
            {
                var averageRating = await reviewService.GetAverageRatingByProductId(productId);
                return Results.Ok(averageRating);
            });

            // Get total reviews by product id
            v1.MapGet("/reviews/product/{productId:guid}/total", async (IReviewService reviewService, Guid productId) =>
            {
                var totalReviews = await reviewService.GetTotalReviewsByProductId(productId);
                return Results.Ok(totalReviews);
            });

            return builder;
        }
    }
}